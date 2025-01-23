using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class FieldsLogic : IFields
    {
        #region Fields

        private readonly ZenithDbContext _context;
        private readonly IRepository<Fields> _fieldsRepository;

        #endregion

        #region Ctor

        public FieldsLogic(ZenithDbContext context, IRepository<Fields> fieldsRepository)
        {
            _context = context;
            _fieldsRepository = fieldsRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Retrieves the value of a column from a data reader or returns the default value if the column is null.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The value of the column or the default value if the column is null.</returns>
        private T GetValueOrDefault<T>(IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return default;

            object value = reader.GetValue(ordinal);

            // Handle special type conversions
            if (typeof(T) == typeof(bool) && value is int intValue)
            {
                return (T)(object)(intValue != 0); // Convert int to bool
            }

            return (T)value; // Default casting
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a list of fields based on the provided filters.
        /// </summary>
        /// <param name="fieldId">The ID of the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="searchText">The search text to filter fields.</param>
        /// <returns>A list of field DTOs.</returns>
        public async Task<List<FieldsDTO>> GetAllfields(Guid? fieldId = null, string fieldName = null, string searchText = null)
        {
            var fieldList = new List<FieldsDTO>();
            try
            {
                var connectionstring = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GETFIELDSDETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (fieldId.HasValue && fieldId != Guid.Empty)
                {
                    command.Parameters.AddWithValue("fieldName", fieldId);
                }

                if (!string.IsNullOrEmpty(fieldName))
                {
                    command.Parameters.AddWithValue("fieldName", fieldName);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    command.Parameters.AddWithValue("SearchText", searchText);
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    fieldList.Add(new FieldsDTO
                    {
                        //SecurityGroupCode = GetValueOrDefault<Guid>(reader, "SecurityGroupCode"),
                        FieldId = GetValueOrDefault<Guid>(reader, "FieldId"),
                        WindowName = GetValueOrDefault<String>(reader, "WindowName"),
                        SectionName = GetValueOrDefault<String>(reader, "SectionName"),
                        TabName = GetValueOrDefault<String>(reader, "TabName"),
                        FieldName = GetValueOrDefault<String>(reader, "FieldName"),
                        //AllowToEdit = GetValueOrDefault<bool>(reader, "AllowToEdit"),
                        //AllowToView = GetValueOrDefault<bool>(reader, "AllowToView"),
                        //AllowToDelete = GetValueOrDefault<bool>(reader, "AllowToDelete"),
                        CreatedBy = GetValueOrDefault<string>(reader, "createdby"),
                        CreatedByName = GetValueOrDefault<string>(reader, "CREATEDBYNAME"),
                        CreatedOn = GetValueOrDefault<DateTime>(reader, "createdon"),
                        ModifiedOn = GetValueOrDefault<DateTime>(reader, "modifiedon"),
                        ModifiedBy = GetValueOrDefault<string>(reader, "modifiedby"),
                        ModifiedByName = GetValueOrDefault<string>(reader, "MODIFIEDBYNAME")
                    });
                }

                return fieldList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Adds a new field.
        /// </summary>
        /// <param name="model">The field DTO.</param>
        public async Task AddFields(FieldsDTO model)
        {
            if (model == null) { throw new ArgumentNullException("model"); }

            var fields = new Fields
            {
                WindowName = model.WindowName,
                //SectionName = model.SectionName,
                FieldName = model.FieldName,
                //AllowToEdit = model.AllowToEdit,
                //AllowToView = model.AllowToView,
                //AllowToDelete = model.AllowToDelete,
                CreatedBy = model.CreatedBy,
                CreatedOn = DateTime.Now
            };

            _fieldsRepository.Add(fields);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a field by its ID.
        /// </summary>
        /// <param name="fieldId">The ID of the field.</param>
        public async Task DeleteField(Guid fieldId)
        {
            var field = await _context.Fields.FirstOrDefaultAsync(x => x.Equals(fieldId));
            if (field != null)
            {
                _context.Fields.Remove(field);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
