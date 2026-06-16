using System;
using System.Collections.Generic;
using System.Text;
using AccountingApp.Api.Models;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AccountingApp.Api
{
    public sealed class AccountingApiService
    {
        private const string BaseUrl = "http://localhost:5265/api/accountentries";

        public async UniTask<List<AccountEntryDto>> GetEntriesAsync()
        {
            using var req = UnityWebRequest.Get(BaseUrl);
            await req.SendWebRequest().ToUniTask();
            ThrowIfError(req);
            return JsonConvert.DeserializeObject<List<AccountEntryDto>>(req.downloadHandler.text);
        }

        public async UniTask<int> CreateEntryAsync(CreateEntryRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            using var req = new UnityWebRequest(BaseUrl, "POST");
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            await req.SendWebRequest().ToUniTask();
            ThrowIfError(req);
            return JsonConvert.DeserializeObject<int>(req.downloadHandler.text);
        }

        public async UniTask UpdateEntryAsync(UpdateEntryRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            using var req = new UnityWebRequest($"{BaseUrl}/{request.Id}", "PUT");
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            await req.SendWebRequest().ToUniTask();
            ThrowIfError(req);
        }

        public async UniTask DeleteEntryAsync(int id)
        {
            using var req = UnityWebRequest.Delete($"{BaseUrl}/{id}");
            req.downloadHandler = new DownloadHandlerBuffer();
            await req.SendWebRequest().ToUniTask();
            ThrowIfError(req);
        }

        private void ThrowIfError(UnityWebRequest req)
        {
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"HTTP {req.responseCode}: {req.error}");
        }
    }
}
