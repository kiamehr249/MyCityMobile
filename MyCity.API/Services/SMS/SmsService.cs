using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel.Proxey;
using MyCiry.ViewModel.SMS;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MyCity.API.Services.SMS {
	public interface ISmsService {
		Task<ApiResult<SmsGetTokenResponse>> GetTokenAsync();
		Task<ApiResult<string>> SendSms(SendSmsRequest entery);
	}

	public class SmsService : ISmsService 
	{
		private readonly IConfiguration _config;

		public SmsService(IConfiguration config) {
			_config = config;
		}

		public async Task<ApiResult<SmsGetTokenResponse>> GetTokenAsync() {
			var settings = _config.GetSection("SMS").Get<SmsSetting>();
			RestClient client = new RestClient("http://RestfulSms.com/api/Token");
			RestRequest request = new RestRequest();
			request.Method = Method.POST;
			request.AddJsonBody(settings);
			var response = await client.ExecuteAsync(request);
			

			var result = new ApiResult<SmsGetTokenResponse> { 
				Status = (int) response.StatusCode
			};

			//if (response.StatusCode != HttpStatusCode.OK) {
			//	result.Content = new SmsGetTokenResponse {
			//		TokenKey = "",
			//		IsSuccessful = false,
			//		Message = response.Content
			//	};
			//	return result;
			//}

			var apiResponse = JsonConvert.DeserializeObject<SmsGetTokenResponse>(response.Content);
			result.Content = apiResponse;
			return result;
		}

		public async Task<ApiResult<string>> SendSms(SendSmsRequest entery) {
			var tokenObj = await GetTokenAsync();
			if ((tokenObj.Status != 200 && tokenObj.Status != 201) || !tokenObj.Content.IsSuccessful) {
				return new ApiResult<string> {
					Status = tokenObj.Status,
					Content = tokenObj.Content != null ? tokenObj.Content.Message : "Sms Token Service have a problem"
				};
			}

			var sendRequest = new SendSmsApiRequest {
				Messages = new List<string> { entery.Text },
				MobileNumbers = new List<string> { entery.Mobile },
				LineNumber = "",
				SendDateTime = "",
				CanContinueInCaseOfError = "false"
			};

			RestClient client = new RestClient("http://RestfulSms.com/api/MessageSend");
			client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", tokenObj.Content.TokenKey));
			RestRequest request = new RestRequest();
			request.Method = Method.POST;
			request.AddJsonBody(sendRequest);
			var response = await client.ExecuteAsync(request);

			var result = new ApiResult<string> {
				Status = (int) response.StatusCode
			};

			//var apiResponse = JsonConvert.DeserializeObject<SmsGetTokenResponse>(response.Content);
			result.Content = response.Content;
			return result;

		}

	}
}
