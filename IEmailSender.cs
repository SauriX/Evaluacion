using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluacion
{
   public  interface IEmailSender
    {
        void SendEmail(Message message);
    }


}
