using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IparticipantMaster
    {
        Task<genericResponse> add(participantMaster clientmaster);
        Task<participantMaster> get(participantMaster clientMaster);
        Task<participantMasterParam> tokenGiftIssuedStatus(participantMaster clientMaster);
        Task<IList<participantMaster>> getList(participantMaster clientMaster);
        Task<genericResponse> tokenIssueStatusUpdate (participantMaster clientMaster);
    }
}
