using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;

namespace Zenith.Controllers
{
    [Authorize]
    public class FieldsController : Controller
    {
        private readonly IFields _fields;

        public FieldsController(IFields fields)
        {
            _fields = fields;
        }

        public async Task<IActionResult> FieldsList(string fieldName = null, string searchText = null)
        {
            var list = await _fields.GetAllfields(null, fieldName, searchText);
            return View(list);
        }

        public async Task<IActionResult> AddField(FieldsDTO model)
        {
            model.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _fields.AddFields(model);
            return View();
        }
        public async Task<IActionResult> GetFieldDetails(Guid fieldId)
        {
            var list = await _fields.GetAllfields(fieldId);
            return View(list.FirstOrDefault());
        }

        public async Task<IActionResult> DeleteField(Guid fieldId)
        {
            await _fields.DeleteField(fieldId);
            return Ok();
        }

        public async Task<IActionResult> SearchFieldsList(string fieldName, string searchText)
        {
            var list = await _fields.GetAllfields(null, fieldName, searchText);

            return View(list); 
        }

        [HttpPost]
        public JsonResult SaveCheckboxData([FromBody] List<FieldsDTO> fieldsData)
        {
            // Process the received data
            foreach (var field in fieldsData)
            {
                // Save data to the database
            }

            return Json(new { success = true, message = "Data saved successfully!" });
        }


    }
}
