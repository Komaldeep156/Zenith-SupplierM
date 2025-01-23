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

        /// <summary>
        /// This method retrieves a list of fields, optionally filtered by field name and search text, and returns the view to display them.
        /// </summary>
        /// <param name="fieldName">The name of the field to filter the results (optional).</param>
        /// <param name="searchText">The text to search within the field (optional).</param>
        /// <returns>Returns the view displaying the list of fields based on the provided filters.</returns>
        public async Task<IActionResult> FieldsList(string fieldName = null, string searchText = null)
        {
            var list = await _fields.GetAllfields(null, fieldName, searchText);
            return View(list);
        }

        /// <summary>
        /// This method handles the addition of a new field. It sets the creator of the field as the logged-in user and then adds the field to the system.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the field to be added.</param>
        /// <returns>Returns the view after the field is added successfully.</returns>
        public async Task<IActionResult> AddField(FieldsDTO model)
        {
            model.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _fields.AddFields(model);
            return View();
        }

        /// <summary>
        /// This method retrieves the details of a specific field based on the provided field ID and returns the view to display the field's details.
        /// </summary>
        /// <param name="fieldId">The unique identifier of the field whose details are to be retrieved.</param>
        /// <returns>Returns the view displaying the details of the specified field.</returns>
        public async Task<IActionResult> GetFieldDetails(Guid fieldId)
        {
            var list = await _fields.GetAllfields(fieldId);
            return View(list.FirstOrDefault());
        }

        /// <summary>
        /// This method handles the deletion of a field by its unique ID. It deletes the specified field and returns a success response.
        /// </summary>
        /// <param name="fieldId">The unique identifier of the field to be deleted.</param>
        /// <returns>Returns an HTTP 200 OK response after the field is deleted successfully.</returns>
        public async Task<IActionResult> DeleteField(Guid fieldId)
        {
            await _fields.DeleteField(fieldId);
            return Ok();
        }

        /// <summary>
        /// This method retrieves a list of fields based on the provided field name and search text, and returns the view to display the filtered results.
        /// </summary>
        /// <param name="fieldName">The name of the field to filter the results (optional).</param>
        /// <param name="searchText">The text to search within the field (optional).</param>
        /// <returns>Returns the view displaying the list of filtered fields based on the provided criteria.</returns>
        public async Task<IActionResult> SearchFieldsList(string fieldName, string searchText)
        {
            var list = await _fields.GetAllfields(null, fieldName, searchText);

            return View(list);
        }
    }
}
