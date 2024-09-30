﻿using Bogus;
using CMS.Data;
using CMS.Entities;
using CMS.Models;
using CMS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft;

namespace CMS.Seed
{
    public static class SeedData
    {

        private static Faker faker = new Faker();
        private static Random random = new Random();

        public static async Task InitAsync(ApplicationDbContext context, ICreateUserService registerService,
            RoleManager<IdentityRole> roleManager)
        {

            if (context.WebSites.Any())
            {
                return;
            }

            //adds templates we have made in folder if we haven't any values in the template table
            if (!context.Templates.Any())
            {
                var templates = CreateTemplates();
                await context.Templates.AddRangeAsync(templates);
            }

            // Seed roles
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var fName = faker.Name.FirstName();
                var lName = faker.Name.LastName();
                var email = faker.Internet.Email(fName, lName, "zocom.se");
                var result = await registerService.CreateUser(email, "pSrkXHN6z8s%KHW@");
                Console.WriteLine(result);
            }

            var result1 = await registerService.CreateUser(@"test@test.com", "pSrkXHN6z8s%KHW@");
            Console.WriteLine(result1);

            var userId = context.Users.Where(u => EF.Functions.Like(u.Email, "test@test.com")).Select(u => u.Id)
                .FirstOrDefault();
            if (userId == null)
            {
                return;
            }

        
            //var websites = CreateWebSites(userId);
            //await context.AddRangeAsync(websites);

            var jsonWebsite = AddJsonWebSites(userId);
            await context.AddRangeAsync(jsonWebsite);

            context.SaveChanges();
        }

        private static ICollection<WebSite> AddJsonWebSites(string OwnerId)
        {
            string json = System.IO.File.ReadAllText("Seed/SeedData.json"); 
            ICollection<WebSite> jsonWebsite = Newtonsoft.Json.JsonConvert.DeserializeObject<ICollection<WebSite>>(json);
            foreach (var webSite in jsonWebsite)
            {
                webSite.UserId = OwnerId;
            }

            return jsonWebsite;
        }

    private static ICollection<WebSite> CreateWebSites(string OwnerId)
        {
            var list = new List<WebSite>();

            for (int i = 0; i < 10; i++)
            {

                var menu = new WebSite
                {
                    Title = faker.Lorem.Slug(2),
                    Description = faker.Lorem.Sentence(),
                    WebPages = CreateWebpages(random.Next(3, 8)),
                    UserId = OwnerId
                };

                list.Add(menu);
            }

            return list;
        }

        private static ICollection<WebPage> CreateWebpages(int count)
        {
            var list = new List<WebPage>();
            for (int i = 0; i < count; i++)
            {

                var webpage = new WebPage
                {
                    Header = faker.Lorem.Slug(2),
                    Body = faker.Lorem.Sentence(10),
                    Footer = faker.Lorem.Sentence(2),
                };

                list.Add(webpage);
            }

            return list;
        }

        private static ICollection<Content> CreateContentBlocks()
        {

            var list = new List<Content>
            {
              new Content
            {
                ContentName = faker.Lorem.Slug(3),
                           },

            new Content
            {

                ContentName ="https://picsum.photos/200/300?random=" + $"{random.Next(1,10000)}",

            },
             new Content
            {
                ContentName = faker.Lorem.Slug(10),

            }
            };

            return list;
        }

        private static ICollection<Template> CreateTemplates()
        {
            var list = new List<Template>
            {
                new Template
                {
                    TemplateType = "Footer2",
                    TemplatePath = "Templates.SingleInput.Template2",
                    InputFormPath = "Templates.InputForm.SingleInputForm"
                },
                new Template
                {
                    TemplateType = "Footer3",
                    TemplatePath = "Templates.SingleInput.Template3",
                    InputFormPath = "Templates.InputForm.SingleInputForm"
                },
                new Template
                {
                    TemplateType = "Footer",
                    TemplatePath = "Templates.SingleInput.Template1",
                    InputFormPath = "Templates.InputForm.DoubleInputForm"
                },
                 new Template
                {
                    TemplateType = "Header",
                    TemplatePath = "Templates.SingleInput.Template1Header",
                    InputFormPath = "Templates.InputForm.DoubleInputForm"
                },
                  new Template
                {
                    TemplateType = "Link",
                    TemplatePath = "Templates.SingleInput.Template1Link",
                    InputFormPath = "Templates.InputForm.DoubleInputForm"
                },
                   new Template
                {
                    TemplateType = "NavBar",
                    TemplatePath = "Templates.SingleInput.Template1NavBar",
                    InputFormPath = "Templates.InputForm.NavBarInputForm"
                },
                   new Template
                {
                    TemplateType = "BodyImageCard",
                    TemplatePath = "Templates.Body.Body1",
                    InputFormPath = "Templates.InputForm.Body1InputForm"
                },
                   new Template
                {
                    TemplateType = "BodySingleImage",
                    TemplatePath = "Templates.Body.SingleImage",
                    InputFormPath = "Templates.InputForm.SingleImageInputForm"
                },
                   new Template
                {
                    TemplateType = "BodySingleVideo",
                    TemplatePath = "Templates.Body.SingleVideoInput",
                    InputFormPath = "Templates.InputForm.SingleVideoInputForm"
                }
            };

            return list;
        }

    }
}