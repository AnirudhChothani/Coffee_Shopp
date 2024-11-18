Step 3: Create the API Model
Models/Employee.cs
namespace MvcCrudApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
    }
}


Step 4: Add an Employee Controller
Controllers/EmployeeController.cs

using MvcCrudApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcCrudApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string apiBaseUrl = "https://api.example.com/employees";

        // GET: Employee
        public async Task<ActionResult> Index()
        {
            List<Employee> employees = new List<Employee>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(jsonString);
                }
            }

            return View(employees);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public async Task<ActionResult> Create(Employee employee)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiBaseUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Employee employee = null;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employee>(jsonString);
                }
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Employee employee)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{apiBaseUrl}/{employee.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(employee);
        }

        // GET: Employee/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Employee employee = null;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employee>(jsonString);
                }
            }
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Employee employee = null;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    employee = JsonConvert.DeserializeObject<Employee>(jsonString);
                }
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync($"{apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
    }
}


Step 5: Create Views for Employee
Views/Employee/Index.cshtml

@model IEnumerable<MvcCrudApp.Models.Employee>
<h2>Employee List</h2>
<table>
    <tr>
        <th>Name</th>
        <th>Position</th>
        <th>Salary</th>
        <th>Actions</th>
    </tr>
    @foreach (var employee in Model)
    {
        <tr>
            <td>@employee.Name</td>
            <td>@employee.Position</td>
            <td>@employee.Salary</td>
            <td>
                @Html.ActionLink("Edit", "Edit", new { id = employee.Id }) |
                @Html.ActionLink("Details", "Details", new { id = employee.Id }) |
                @Html.ActionLink("Delete", "Delete", new { id = employee.Id })
            </td>
        </tr>
    }
</table>
<a href="@Url.Action("Create")">Create New</a>


Views/Employee/Edit.cshtml

@model MvcCrudApp.Models.Employee

<h2>Edit Employee</h2>

@using (Html.BeginForm("Edit", "Employee", FormMethod.Post))
{
    @Html.HiddenFor(model => model.Id)

    <div>
        <label>Name:</label>
        @Html.TextBoxFor(model => model.Name, new { @class = "form-control" })
    </div>
    <div>
        <label>Position:</label>
        @Html.TextBoxFor(model => model.Position, new { @class = "form-control" })
    </div>
    <div>
        <label>Salary:</label>
        @Html.TextBoxFor(model => model.Salary, new { @class = "form-control" })
    </div>
    <br />
    <button type="submit" class="btn btn-primary">Save Changes</button>
    @Html.ActionLink("Back to List", "Index", null, new { @class = "btn btn-secondary" })
}




Views/Employee/Details.cshtml
@model MvcCrudApp.Models.Employee

<h2>Employee Details</h2>

<div>
    <strong>Name:</strong> @Model.Name
</div>
<div>
    <strong>Position:</strong> @Model.Position
</div>
<div>
    <strong>Salary:</strong> @Model.Salary
</div>

<br />
@Html.ActionLink("Back to List", "Index", null, new { @class = "btn btn-secondary" })
@Html.ActionLink("Edit", "Edit", new { id = Model.Id }, new { @class = "btn btn-primary" })


Delete View
@model MvcCrudApp.Models.Employee

<h2>Delete Employee</h2>

<div>
    <strong>Are you sure you want to delete this employee?</strong>
</div>
<div>
    <p><strong>Name:</strong> @Model.Name</p>
    <p><strong>Position:</strong> @Model.Position</p>
    <p><strong>Salary:</strong> @Model.Salary</p>
</div>

@using (Html.BeginForm("Delete", "Employee", FormMethod.Post))
{
    @Html.HiddenFor(model => model.Id)

    <button type="submit" class="btn btn-danger">Delete</button>
    @Html.ActionLink("Cancel", "Index", null, new { @class = "btn btn-secondary" })
}


appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApiSettings": {
    "BaseUrl": "https://api.example.com/employees"
  }
}



Program.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MvcCrudApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


Startup.cs
In ASP.NET Core MVC, the Startup.cs file is used to configure services and the HTTP request pipeline.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcCrudApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Register API Base URL from appsettings.json
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new System.Uri(Configuration["ApiSettings:BaseUrl"]);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}



Accessing the API Base URL in the Controller
To use the API base URL from appsettings.json, you can inject IConfiguration into the controller:

Updated EmployeeController.cs
using Microsoft.Extensions.Configuration;
using MvcCrudApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcCrudApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string _apiBaseUrl;

        public EmployeeController()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        // (Other actions remain the same, replace the hardcoded API URL with _apiBaseUrl)
    }
}
