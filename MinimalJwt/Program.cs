using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalJwt.Models;
using MinimalJwt.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalJwt
{
    public class Program
    {
        static WebApplicationBuilder builder;
        public static void Main(string[] args)
        {
            builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme="Bearer",
                    BearerFormat="JWT",
                    In=ParameterLocation.Header,
                    Name="Authorization",
                    Description="Bearer Authentication with JWT Token",
                    Type=SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {{
                    new OpenApiSecurityScheme()
                    {
                       Reference=new OpenApiReference()
                       {
                           Id="Bearer",
                           Type=ReferenceType.SecurityScheme
                       }
                    },
                    new List<string>()
                } });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<IMovieService, MovieService>();
            builder.Services.AddTransient<IUserService, UserService>();


            var app = builder.Build();
            app.UseSwagger();
            app.UseAuthorization();
            app.UseAuthentication();

            app.MapGet("/", () => "Hello World!");
            
            app.MapPost("/login", ([FromBody] UserLogin user, IUserService service) => Login(user, service));

            app.MapPost("/create",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
            ([FromBody] Movie movie, [FromServices] IMovieService service) => Create(movie, service));
            
            app.MapPost("/get",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, Standart")]
            ([FromBody] int id, [FromServices] IMovieService service) => Get(id, service));
          
            app.MapGet("/list", ([FromServices] IMovieService service) => GetAll(service));

            app.MapPut("/update",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
            ([FromBody] Movie newMovie, [FromServices] IMovieService service) => Update(newMovie, service));
           
            app.MapDelete("/delete",
               [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
            ([FromBody] int id, [FromServices] IMovieService service) => Delete(id, service));

            app.UseSwaggerUI();
            app.Run();
        }

        private static IResult Login(UserLogin user, IUserService service)
        {
            var loggedUser = service.Get(user);
            if (loggedUser is null) return Results.NotFound("User not registered");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,loggedUser.UserName),
                new Claim(ClaimTypes.Email, loggedUser.EmailAddress),
                new Claim(ClaimTypes.GivenName,loggedUser.GivenName),
                new Claim(ClaimTypes.Surname,loggedUser.Surname),
                new Claim(ClaimTypes.Role,loggedUser.Role)
            };


            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (
                issuer: builder.Configuration["Jwt:Issuer"],
                audience: builder.Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                notBefore: DateTime.Now,
                signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(tokenString);
        }

        private static IResult Delete(int id, IMovieService service)
        {
            bool result = service.Delete(id);
            if (result)
                return Results.Ok("Deleted");

            return Results.Problem("Something went wrong");
        }

        private static IResult Update(Movie newMovie, IMovieService service)
        {
            var result = service.Update(newMovie);

            if (result is null)
                return Results.Problem("Something went wrong");

            return Results.Ok($"updated: {result}");
        }

        private static IResult GetAll(IMovieService service)
        {
            return Results.Ok(service.GetAll());
        }

        static IResult Create(Movie movie, IMovieService service)
        {
            var result = service.Create(movie);

            return Results.Ok(result);
        }

        static IResult Get(int id, IMovieService service)
        {
            var result = service.Get(id);

            if (result is null) return Results.NotFound("movie not found");
            return Results.Ok(result);
        }




    }
}