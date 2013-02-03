using System;
using System.Configuration;
using System.Globalization;
using Nancy;
using Newtonsoft.Json.Linq;
using Open311API;
using Open311API.Exception;
using Open311API.Structs;
using Twilio;

public class GetStatusModule : NancyModule
{
    // 311 API endpoint.
    const string ENDPOINT = "http://www.publicstuff.com/api/open311/";

    // Jursdiction identifier to use with API calls.
    const string JURISDICTION_ID = "philadelphia-pa";

    //Twilio credentials.
    private string ACCOUNTSID = ConfigurationManager.AppSettings["TwilioAccountSID"].ToString();
    private string AUTHTOKEN = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
    private string FROMNUMBER = ConfigurationManager.AppSettings["TwilioFromNumber"].ToString();

    // Used for formatting response to SMS gateway.
    private string response;
    private TextInfo ti = new CultureInfo("en-US", false).TextInfo;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetStatusModule()
    {
        // Create new Open311 object instance.
        var report = new Open311(ENDPOINT, JURISDICTION_ID);

        // Create new Twilio object instance.
        var twilio = new TwilioRestClient(ACCOUNTSID, AUTHTOKEN);

        // Route for POST from SMS gateway.
        Get["/getStatus"] = x =>
        {
            // Users callerID.
            string userNumber = Request.Query["From"];

            // Message sent with inbound SMS.
            string serviceRequestID = Request.Query["Body"];

            if (isNumber(serviceRequestID))
            {
                try
                {

                    JArray statusJson = JArray.Parse(report.GetServiceRequest(ResponseFormat.JSON, serviceRequestID));
                    response = FormatResponse(statusJson);
                }
                catch (Open311Exception ex)
                {
                    response = "Sorry, I did not find that service request. :-(";
                }
            }
            else
            {
                response = "Sorry, I could not look up service request ID " + serviceRequestID + ".";
            }
            twilio.SendSmsMessage(FROMNUMBER, userNumber, response);
            return 200;
        };
    }

    /// <summary>
    /// Check to ensure that SMS body contains a valid numeric service ID.
    /// </summary>
    /// <param name="num">User suibmitted ID to be checked.</param>
    /// <returns>bool</returns>
    private bool isNumber(string num)
    {
        try
        {
            int id = Int32.Parse(num.Trim());
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }        
    }
    
    /// <summary>
    /// Format SMS response.
    /// </summary>
    /// <param name="statusJson">JSON response from 311 API.</param>
    /// <returns>Formatted response</returns>
    private string FormatResponse(JArray statusJson)
    {
        response = "Service request: " + statusJson[0]["service_request_id"].ToString() + "\n";
        response += "Type: " + statusJson[0]["service_name"].ToString() + "\n";
        response += "Reported: " + statusJson[0]["requested_datetime"].ToString() + "\n";
        response += "Status: " + ti.ToTitleCase(statusJson[0]["status"].ToString()) + "\n";
        return response;
    }
}