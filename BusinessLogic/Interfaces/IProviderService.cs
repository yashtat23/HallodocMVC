using DataAccess.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IProviderService
    {
        void acceptCase(int requestId,string loginUserId);

        bool TransferRequest(TransferRequest model);
    }
}
