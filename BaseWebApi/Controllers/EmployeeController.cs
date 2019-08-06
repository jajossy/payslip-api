using BaseWebApi.Models;
using BaseWebApi.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class EmployeeController : ApiController
    {
        private readonly IGenericRepository<Employee> _employeeRepository;

        public EmployeeController(IGenericRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IQueryable<Employee> Get()
        {
            return _employeeRepository.GetAll();
        }
    }
}
