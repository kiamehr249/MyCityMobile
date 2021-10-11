using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel.Proxey;
using MyCiry.ViewModel.SMS;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MyCity.API.Services.SMS {
	public interface ISmsService {
		Task<ApiResult<SmsGetTokenResponse>> GetTokenAsync();
		Task<ApiResult<string>> GetSmsLines();
		Task<ApiResult<SmsGetTokenResponse>> SendSms(SendSmsRequest entery);
	}

	public class SmsService : ISmsService {
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

		public async Task<ApiResult<string>> GetSmsLines() {
			var result = new ApiResult<string>();
			var tokenObj = await GetTokenAsync();
			if (tokenObj == null) {
				return new ApiResult<string> {
					Status = 401,
					Content = "token gozid"
				};
			} else if ((tokenObj.Status != 200 && tokenObj.Status != 201) || !tokenObj.Content.IsSuccessful) {
				return new ApiResult<string> {
					Status = tokenObj.Status,
					Content = tokenObj.Content.Message
				};
			}

			try {
				RestClient client = new RestClient("http://RestfulSms.com/api/SMSLine");
				client.AddDefaultHeader("x-sms-ir-secure-token", $"{tokenObj.Content.TokenKey}");
				RestRequest request = new RestRequest();
				request.Method = Method.GET;
				//request.AddJsonBody(sendRequest);
				var response = await client.ExecuteAsync(request);

				result.Status = (int) response.StatusCode;
				result.Content = "";

				//var apiResponse = JsonConvert.DeserializeObject<SmsGetTokenResponse>(response.Content);
				result.StrResult = response.Content;
				return result;
			} catch (Exception ex) {
				result.StrResult = ex.Message;
				result.Status = 500;
				return result;
			}




		}

		public async Task<ApiResult<SmsGetTokenResponse>> SendSms(SendSmsRequest entery) {
			var tokenObj = await GetTokenAsync();
			if ((tokenObj.Status != 200 && tokenObj.Status != 201) || !tokenObj.Content.IsSuccessful) {
				return new ApiResult<SmsGetTokenResponse> {
					Status = tokenObj.Status,
					Content = tokenObj.Content
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
			client.AddDefaultHeader("x-sms-ir-secure-token", string.Format("{0}", tokenObj.Content.TokenKey));
			RestRequest request = new RestRequest();
			request.Method = Method.POST;
			request.AddJsonBody(sendRequest);
			var response = await client.ExecuteAsync(request);

			var result = new ApiResult<SmsGetTokenResponse> {
				Status = (int) response.StatusCode
			};

			var apiResponse = JsonConvert.DeserializeObject<SmsGetTokenResponse>(response.Content);
			result.Content = apiResponse;
			return result;

		}

	}
}
