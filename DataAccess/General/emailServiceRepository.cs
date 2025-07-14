using MailKit;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Models;
using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static Models.ClsGloble;

namespace DataAccess.General
{
    public class emailServiceRepository : IemailService
    {
        private readonly IemailLog _iemailLog;
        private readonly IemailConfiguration _iemailConfig;
        private emailLogModel emailLog = new emailLogModel();
        public emailServiceRepository(IemailLog IemailLog, 
            IemailConfiguration iemailConfig
           )
        {
            _iemailLog = IemailLog;
            _iemailConfig = iemailConfig;
        }
        public async Task<genericResponse> SendEMailAsync(emailServiceModel mailRequest)
        {
            var response = new genericResponse();
            try
            {
                var emailConfigParam = new emailConfigurationModel
                {
                    serviceType = (int)mailRequest.serviceType, // (int)enumEmailServiceType.GeneralService, 
                    isDefault = true,
                };
                var emailConfiguration = await _iemailConfig.get(emailConfigParam);
                if (emailConfiguration != null)
                {
                    mailRequest.ishtml = true;
                    response = await SendAsync(emailConfiguration, mailRequest);
                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "Host Email Configuration does not found";
                }
            }
            catch (Exception ex)
            {
                response.success = (int)enumResponseCode.error;
                response.message = ex.Message.ToString();
            }
            return response;
        }
        private async Task<genericResponse> SendAsync(emailConfigurationModel emailConfiguration, emailServiceModel mailRequest)
        {
            var response = new List<MailResponse>();
            var success = (int)enumResponseCode.success;

            if (mailRequest.recipents != null && mailRequest.recipents.Count != 0)
            {
                var mimeMessage = new MimeMessage();

                //MailResponse
                foreach (var recipent in mailRequest.recipents)
                {
                    try
                    {
                        /* To */
                        mimeMessage.To.Add(MailboxAddress.Parse(recipent));

                        /* Cc*/
                        if (mailRequest.ccList != null && mailRequest.ccList.Count != 0)
                        {
                            foreach (var item in mailRequest.ccList)
                            {
                                mimeMessage.Cc.Add(MailboxAddress.Parse(item));
                            }
                        }
                        /* Bcc */
                        if (mailRequest.bccList != null && mailRequest.bccList.Count != 0)
                        {
                            foreach (var item in mailRequest.bccList)
                            {
                                mimeMessage.Bcc.Add(MailboxAddress.Parse(item));
                            }
                        }

                        mimeMessage.Subject = mailRequest.subject;
                        mimeMessage.Body = new TextPart(mailRequest.ishtml ? TextFormat.Html : TextFormat.Plain)
                        {
                            Text = mailRequest.body
                        };


                        MailboxAddress mailFrom = new MailboxAddress(emailConfiguration.displayName, emailConfiguration.hostUser);                        
                        /* Email sending config */
                        mimeMessage.From.Add(mailFrom); // MailboxAddress.Parse(emailConfiguration.hostUser)

                        if (emailConfiguration.hostType.Equals((int)enumEmailHostType.smtp))
                        {
                            //send email via smtp configuration
                            using (var smtp = new MailKit.Net.Smtp.SmtpClient(new ProtocolLogger("smtp.log")))
                            {
                                var emailLogModel = new emailLogModel
                                {
                                    parentId = mailRequest.parentId,
                                    parentType = mailRequest.parentType,
                                    hostMail = emailConfiguration.hostUser,//mailRequest.hostMail,
                                    toMail = recipent,
                                    sentTime = DateTime.Now,
                                    status = false,
                                    subject = mailRequest.subject
                                };

                                //smtp.MessageSent += new EventHandler<MessageSentEventArgs>((sender, e) => OnMailSent(sender, e));                        
                                //smtp.ServerCertificateValidationCallback = (s, c, h, e) => false;
                                emailLog.parentId = mailRequest.parentId;
                                emailLog.parentType = mailRequest.parentType;
                                emailLog.hostMail = mailRequest.hostMail;
                                emailLog.toMail = recipent;
                                emailLog.subject = mailRequest.subject;

                                smtp.MessageSent += onMailSent; //new EventHandler<MessageSentEventArgs>(onMailSent);
                                smtp.SslProtocols = (SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13);
                                await smtp.ConnectAsync(emailConfiguration.hostServer, emailConfiguration.port, SecureSocketOptions.StartTls);
                                await smtp.AuthenticateAsync(emailConfiguration.userName, emailConfiguration.password);
                                await smtp.SendAsync(mimeMessage);
                                await smtp.DisconnectAsync(true);

                                response.Add(new MailResponse { status = true, message = string.Concat(mimeMessage.To, ":", "Email sent.") });
                            }
                        }
                        else if (emailConfiguration.hostType.Equals(enumEmailHostType.googleWorkspaceEmail))
                        {
                            // send email via google workSpace configuration
                        }
                    }
                    catch (Exception ex)
                    {
                        emailLog.sentTime = DateTime.Now;
                        emailLog.status = false;

                        response.Add(new MailResponse
                        {
                            status = false,
                            message = ex.Message
                        });
                    }
                }
            }
            return new genericResponse { success = success, data = response };
        }
        private void onMailSent(object sender, MessageSentEventArgs e)
        {
            emailLog.sentTime = DateTime.Now;
            emailLog.status = e.Message.MessageId != null;

            _iemailLog.add(emailLog);

            // Write email log event
            Console.WriteLine("The message was sent!");
        }
    }
}
