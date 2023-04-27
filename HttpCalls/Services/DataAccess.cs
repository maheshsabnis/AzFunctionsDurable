using HttpCalls.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpCalls.Services
{
    public class DepartmentService : IServices<Department, int>
    {
        private readonly CompanyContext _context;
        private ResponseObject<Department> _response;
        public DepartmentService(CompanyContext context)
        {
            _context = context; 
            _response = new ResponseObject<Department>();
        }

        async Task<ResponseObject<Department>> IServices<Department, int>.CreateAsync(Department entity)
        {
            var record = await _context.Departments.AddAsync(entity);
            await _context.SaveChangesAsync();
            _response.Record = record.Entity;
            _response.StatusMessage = $"Department is added succssfully";
            _response.StatusCode = 201;
            return _response;
        }

        async Task<ResponseObject<Department>> IServices<Department, int>.DeleteAsync(int id)
        {
            var record = await _context.Departments.FindAsync(id);
            if (record == null)
            {
                _response.Record = null;
                _response.StatusMessage = $"Department is not found";
                _response.StatusCode = 401;
            }
            else
            {
                _context.Departments.Remove(record);
                await _context.SaveChangesAsync();
                _response.StatusMessage = $"Department is deleted succssfully";
                _response.StatusCode = 202;
            }
            return _response;
        }

        async Task<ResponseObject<Department>> IServices<Department, int>.GetAsync()
        {
            try
            {

                var records = await _context.Departments.ToListAsync();
                _response.Records = records;
                _response.StatusMessage = "Departments are received successfully";
                _response.StatusCode = 200;
                return _response;
            }
            catch (Exception ex)
            {

                _response.StatusMessage = ex.Message;
                _response.StatusCode = 200;
                return _response;

            }
        }

        async Task<ResponseObject<Department>> IServices<Department, int>.GetAsync(int id)
        {
            var record = await _context.Departments.FindAsync(id);
            if (record == null)
            {
                _response.Record = null;
                _response.StatusMessage = $"Department is not found";
                _response.StatusCode = 401;
            }
            else
            {
                _response.Record = record;
                _response.StatusMessage = $"Department is found succssfully";
                _response.StatusCode = 200;
            }
            return _response;
        }

        async Task<ResponseObject<Department>> IServices<Department, int>.UpdateAsync(int id, Department entity)
        {
            var record = await _context.Departments.FindAsync(id);
            if (record == null)
            {
                _response.Record = null;
                _response.StatusMessage = $"Department is not found";
                _response.StatusCode = 401;
            }
            else
            {
                record.DeptName = entity.DeptName;
                record.Location = entity.Location;
                record.Capacity = entity.Capacity;
                await _context.SaveChangesAsync();
                _response.Record = record;
                _response.StatusMessage = $"Department is upated succssfully";
                _response.StatusCode = 202;
            }
            return _response;
        }
    }
}
