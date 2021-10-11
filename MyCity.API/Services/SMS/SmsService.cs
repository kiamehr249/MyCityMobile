using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel.Proxey;
using MyCiry.ViewModel.SMS;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyCity.API.Services.SMS {
	public interface ISmsService {
		Task<ApiResult<SmsToken>> GetTokenAsync();
		Task<ApiResult<SmsLineResponse>> GetSmsLines();
		Task<ApiResult<BaseSmsResponse>> SendSms(SendSmsRequest entery);
	}

	public class SmsService : ISmsService {
		private readonly IConfiguration _config;

		public SmsService(IConfiguration config) {
			_config = config;
		}

		public async Task<ApiResult<SmsToken>> GetTokenAsync() {
			var settings = _config.GetSection("SMS").Get<SmsSetting>();
			RestClient client = new RestClient("http://RestfulSms.com/api/Token");
			RestRequest request = new RestRequest();
			request.Method = Method.POST;
			request.AddJsonBody(settings);
			var response = await client.ExecuteAsync(request);

			var result = new ApiResult<SmsToken> {
				Status = (int) response.StatusCode
			};

			if ((int)response.StatusCode != 200 && (int)response.StatusCode != 201) {
				result.Content = new SmsToken {
					TokenKey = "",
					IsSuccessful = false,
					Message = response.Content
				};
				return result;
			}

			var apiResponse = JsonConvert.DeserializeObject<SmsToken>(response.Content);
			result.Content = apiResponse;
			return result;
		}

		public async Task<ApiResult<SmsLineResponse>> GetSmsLines() {
			var result = new ApiResult<SmsLineResponse>();
			var tokenObj = await GetTokenAsync();
			if (tokenObj == null) {
				result.Status = 401;
				result.StrResult = "Token Problem";
				result.Content = new SmsLineResponse();
				return result;
			} else if ((tokenObj.Status != 200 && tokenObj.Status != 201) || !tokenObj.Content.IsSuccessful) {
				result.Status = tokenObj.Status;
				result.StrResult = tokenObj.Content.Message;
				result.Content = new SmsLineResponse();
				return result;
			}

			RestClient client = new RestClient("http://RestfulSms.com/api/SMSLine");
			client.AddDefaultHeader("x-sms-ir-secure-token", $"{tokenObj.Content.TokenKey}");
			RestRequest request = new RestRequest();
			request.Method = Method.GET;
			//request.AddJsonBody(sendRequest);
			var response = await client.ExecuteAsync(request);

			result.Status = (int) response.StatusCode;

			var apiResponse = JsonConvert.DeserializeObject<SmsLineResponse>(response.Content);
			result.Content = apiResponse;
			result.StrResult = response.Content;
			return result;

		}

		public async Task<ApiResult<BaseSmsResponse>> SendSms(SendSmsRequest entery) {
			var tokenObj = await GetTokenAsync();
			if ((tokenObj.Status != 200 && tokenObj.Status != 201) || !tokenObj.Content.IsSuccessful) {
				return new ApiResult<BaseSmsResponse> {
					Status = tokenObj.Status,
					Content = tokenObj.Content
				};
			}

			var smslines = await GetSmsLines();
			if (smslines == null) {
				return new ApiResult<BaseSmsResponse> {
					Status = 500,
					Content = new BaseSmsResponse { 
						Message = "Lines Problem",
						IsSuccessful = false
					},
					StrResult = ""
				};
			} else if (!smslines.Content.IsSuccessful || smslines.Content.SMSLines.Count == 0) {
				return new ApiResult<BaseSmsResponse> {
					Status = smslines.Status,
					Content = new BaseSmsResponse {
						Message = smslines.Content.Message,
						IsSuccessful = false
					},
					StrResult = ""
				};
			}

			var sendRequest = new SendSmsApiRequest {
				Messages = new List<string> { entery.Text },
				MobileNumbers = new List<string> { entery.Mobile },
				LineNumber = smslines.Content.SMSLines.FirstOrDefault().LineNumber.ToString(),
				SendDateTime = "",
				CanContinueInCaseOfError = "false"
			};

			RestClient client = new RestClient("http://RestfulSms.com/api/MessageSend");
			client.AddDefaultHeader("x-sms-ir-secure-token", string.Format("{0}", tokenObj.Content.TokenKey));
			RestRequest request = new RestRequest();
			request.Method = Method.POST;
			request.AddJsonBody(sendRequest);
			var response = await client.ExecuteAsync(request);

			var result = new ApiResult<BaseSmsResponse> {
				Status = (int) response.StatusCode
			};

			var apiResponse = JsonConvert.DeserializeObject<BaseSmsResponse>(response.Content);
			result.Content = apiResponse;
			result.StrResult = response.Content;
			return result;

		}

	}
}
