using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.ClsGloble;

namespace Models.General
{
    public class emailServiceModel
    {
        public string hostMail { get; set; }
        public string displayName { get; set; }
        public List<string> recipents { get; set; } = new List<string>();
        public List<string> ccList { get; set; } = new List<string>();
        public List<string> bccList { get; set; } = new List<string>();
        public string subject { get; set; }
        public string body { get; set; }
        public bool ishtml { get; set; } = false;
        public int? parentId { get; set; }
        public int? parentType { get; set; }
        public enumEmailServiceType? serviceType { get; set; }
       // public List<AttachmentEntity> Attachments { get; set; } = new List<AttachmentEntity>();
    }
    public class mailRequest
    {
        public string from { get; set; }
        public string displayname { get; set; }
        public List<string> Recipents { get; set; } //to        
        public List<string> BccList { get; set; } = new List<string>();
        public string subject { get; set; }
        public string body { get; set; }
        public int? parentid { get; set; }
        public string parenttype { get; set; }
        public bool ishtml { get; set; } = false;
        public bool IsExistAttachment { get; set; } = false;
        public string unsubscribeLink { get; set; }
        //public IFormFile Attachments { get; set; } = null;    

    }
    public class attachmentEntity
    {
        public string fileName { get; set; }
        public byte[] data { get; set; }
        public string contentType { get; set; }
    }
    public class MailResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
}
