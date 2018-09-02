using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ICSharpCode.SharpZipLib.Zip;
using Wiki.PriceSender.Service.Models.PriceList;
using Wiki.PriceSender.Dto;

namespace Wiki.PriceSender.Service
{
    public class MailSender
    {


        private EmailSetting setting;

        public void SendEmail(string to, string subject, string body, bool isHtml, byte[] attachFile,string fileName, EmailSetting emailSetting
            /*, params string[] filenames*/)
        {

            if (string.IsNullOrWhiteSpace(to) || to.Trim().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray().Length == 0)
            {
                SenderSrv.Logger.WriteEvent(string.Format("Email not sended. To is empty. subject:{0}, fileName:{1}", subject, fileName));
                return;
            }
            string[] toMass =
                to.Trim()
                    .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToArray();

            var setting = emailSetting;
            using (SmtpClient smtp = new SmtpClient(setting.SmtpServer, setting.SmtpPort))
            {
                if (setting.UseSsl)
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;

                    smtp.Credentials = new NetworkCredential(setting.Login, setting.Password);
                }
                MailMessage message = new MailMessage();
                message.From = new MailAddress(setting.Email);
                foreach (string To in toMass)
                    message.To.Add(To.Trim());
                message.Subject = subject;
                message.IsBodyHtml = isHtml;
                message.Body = body;
                if (attachFile != null)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(this.ConvertToZip(attachFile, fileName)),this.ClearName(fileName) + ".zip"));
                }

                try
                {
                    smtp.Send(message);
                    SenderSrv.Logger.WriteEvent("Email sended. From:{0}, to:{1},subject:{2}", message.From, to, subject);
                }
                catch (Exception e)
                {
                    SenderSrv.Logger.WriteError(
                        string.Format("Error send mail. From:{0}, to:{1},subject:{2}", message.From, to, subject), e);
                    throw;
                }
            }
        }

        private byte[] ConvertToZip(byte[] pr, string fileName)
        {
            byte[] fileBytes = pr;
            byte[] compressedBytes;
            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    using (var entryStream = fileInArchive.Open())
                    using (var fileToCompressStream = new MemoryStream(fileBytes))
                    {
                        fileToCompressStream.CopyTo(entryStream);
                    }
                }
                compressedBytes = outStream.ToArray();
            }
            return compressedBytes;
        }

        private string ClearName(string fullName)
        {
            try
            {
                int index = fullName.LastIndexOf(".");
                if (index > 0)
                    return fullName.Substring(0, index);
                else
                {
                    return fullName;
                }
            }
            catch (Exception)
            {
                
                return fullName;
            }
        }

        public const string OrderBodyTemplate=
@"

    <div style='background-color:#ffffff; text-align:center;'>
        <table align='center' width='800'>
            <tr>
                <td style='text-align:left;'>
                    <a href='http://www.rusimport.com' target='_blank' style='text-decoration: none'><img style='outline: none; border: none;' width='317' height='79' src='http://rusimport.com/themes/rusimport/img/dizign/logo_mail.jpg' /></a>
                    <span style='text-align:center;font-size:0px;color:#ffffff;'>������ ����� �����</span>
                </td>
                <td align='right' valign='bottom'>
                    <span style='font-family:Arial;text-align:right;font-size:14px;color:#bbbbbb;'>{0}</span><br />
                    <span style='font-family:Arial;text-align:right;font-size:12px;color:darkred;margin-top: 50px;'>��� ������ ������������� �������������</span>
                </td>
            </tr>
        </table>
        <hr color='#af0025' size='1' style='padding:0; margin:0;' />
        <table align='center' width='800'>
            <tr>
                <td style='text-align:left;'>
                    <table align='center' style='font-size: 13px; font-family:Arial; width: 800px; border-spacing:0; border-collapse:collapse;' cellspacing='0' cellpadding='0'>
                        <tr><td colspan='2' style='padding-top:15px;'><b>��������� ��������� � {1} {2}</b></td></tr>
                        <tr><td colspan='2' style='padding-top: 10px;'>������� ���������� ����� <b>� {3}</b></td></tr>
                        <tr><td colspan='2' style='text-align:center; padding-top: 20px; color: darkred'><b>������������ ������� ������� ���� ��� ���������� ������.</b></td></tr>
                        <tr><td colspan='2' style='text-align: center; color: darkred'><b>� ������ ����������� �� ���������� � ���� ������ ������������ ��������� ������.</b></td></tr>
                        <tr><td colspan='2' style='padding-top: 15px; color: darkred'><b style='color: darkred;'>������ ������:</b></td></tr>
                    </table>
                    <table align='center' style='font-size: 12px; font-family:Arial; width: 800px; border-left: 1px solid #A6BCCB; border-top: 1px solid #A6BCCB; border-spacing:0; border-collapse:collapse;' cellspacing='0' cellpadding='0'>
                        <tbody>
                            <tr style='background-color:#dcdcdc; '>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'>�����</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB;border-right: 1px solid #A6BCCB;'>�������</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB;border-right: 1px solid #A6BCCB;'>������������</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'>���-��</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'>���� (���.)</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'>��������� (���.)</th>
                                <th style='padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'>�������</th>
                            </tr>
                            {4}
                            <tr style='background-color:#dcdcdc;'>
                                <td colspan=5 style='white-space:nowrap; text-align:right; padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'><b>�����:</b></td>
                                <td style='white-space:nowrap; text-align:right; padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'><b>{5}</b></td>
                                <td style='white-space:nowrap; text-align:right; padding:2px 5px; border-bottom: 1px solid #A6BCCB; border-right: 1px solid #A6BCCB;'></td>
                            </tr>
                        </tbody>
                    </table>
                    <font size='2' face='Arial' color='#000'>
                        <br /><br />
                        <table align='center' style='font-size: 12px; font-family:Arial; width: 800px; border-spacing:0;'>
                            <tr><td colspan='2' style='padding-top: 10px;'><b>�� ������ ���������� ������ ���������� ���������:</b></td></tr>
                            <tr><td colspan='2' style='padding-left:10px;padding-top:5px; '>1. � ����������� � ������� ������ ����� � ������� ��� �� zakaz@rusimport.com</td></tr>
                            <tr><td colspan='2' style='vertical-align:middle; height:32px;'>
                                    <table border=0 style='border-collapse: collapse; float: left; margin-top: 10px;'>
                                        <tr>
                                            <td valign='middle' height='32' style='padding: 0 10px; text-align: center;'>
                                                2. �� ����� �����:
                                            </td>
                                            <td>
                                                <table background='http://rusimport.com/themes/rusimport/img/dizign/button_mail.png' bgcolor='#3da909' border=0 style='border-collapse: collapse; margin-left: 10px; float: left;'>
                                                    <tr>
                                                        <td valign='middle' height='32' style='padding: 0 10px; text-align: center;'>
                                                            <a href='http://rusimport.com/suppliers/orders/?id={6}' style='color: #ffffff;  font-size: 14px;  text-decoration: none; font-weight: bold;'>���������� ������</a>
                                                        </td>
                                                   </tr>
                                               </table>
                                            </td>
                                        </tr>
                                    </table>
                            </td></tr>
                            <tr><td colspan='2' style='padding-top: 20px;'><b>� ���������,</b></td></tr>
                            <tr><td colspan='2' ><b>��������� �� ������ � �������� ��������</b></td></tr>
                        </table>
                    </font>
                </td>
            </tr>
        </table>
        <hr color='#696969' size='1' style='padding: 0; margin: 10px 0;' />
        <table align='center' width='800'>
            <tr>
                <td style='text-align:left;'>
                    <font size='2' face='Tahoma' color='#696969'>
                        ������� �� ��������������<br />
                        �������� <b>&laquo;����������������� {7}&raquo;</b><br />
                        ������, �. �����-���������, ��. ���������������, 3<br />
                        ���: +7(812) 303-93-23 ���. 114, 130<br />
                        Email: zakaz@rusimport.com<br />
                        <a href='http://rusimport.com' style='color:#1e4fb4;' target='_blank'>www.rusimport.com</a>
                    </font>
                </td>
            </tr>
        </table> 
    </div>


";
    }

}