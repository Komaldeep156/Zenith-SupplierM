using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.BLL.Interface
{
    public interface ISetting
    {
        public Task<string> ChangePassword(string currentPassword, string newPassword, string loggedInUserId);
    }
}
