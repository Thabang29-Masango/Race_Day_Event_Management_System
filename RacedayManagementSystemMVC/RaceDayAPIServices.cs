using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDayManagementSystemMVC.Models;

namespace RaceDayManagementSystemMVC.Services;

public class RaceDayApiService
{
    private readonly HttpClient _http;
    public RaceDayApiService(HttpClient http) => _http = http;

    public Task<ApiResult<LoginResponse>> LoginAsync(string email, string password) =>
        SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/login", new { email, password });

    public Task<ApiResult<object>> RegisterAsync(RegisterViewModel model) =>
        SendAsync<object>(HttpMethod.Post, "api/auth/register", model);

    public Task<ApiResult<List<EventDto>>> GetEventsAsync() => GetAsync<List<EventDto>>("api/events");
    public Task<ApiResult<EventDto>> GetEventAsync(int id) => GetAsync<EventDto>($"api/events/{id}");
    public Task<ApiResult<EventDto>> CreateEventAsync(EventDto value) => SendAsync<EventDto>(HttpMethod.Post, "api/events", value);
    public Task<ApiResult<object>> UpdateEventAsync(int id, EventDto value) => SendAsync<object>(HttpMethod.Put, $"api/events/{id}", value);
    public Task<ApiResult<object>> DeleteEventAsync(int id) => SendAsync<object>(HttpMethod.Delete, $"api/events/{id}");

    public Task<ApiResult<List<CategoryDto>>> GetCategoriesAsync(int eventId) => GetAsync<List<CategoryDto>>($"api/events/{eventId}/categories");
    public Task<ApiResult<CategoryDto>> CreateCategoryAsync(int eventId, CategoryDto value) => SendAsync<CategoryDto>(HttpMethod.Post, $"api/events/{eventId}/categories", value);
    public Task<ApiResult<object>> DeleteCategoryAsync(int categoryId) => SendAsync<object>(HttpMethod.Delete, $"api/categories/{categoryId}");

    public Task<ApiResult<List<EnrolmentDetailDto>>> GetEventEnrolmentsAsync(int eventId) => GetAsync<List<EnrolmentDetailDto>>($"api/events/{eventId}/enrolments");
    public Task<ApiResult<List<EnrolmentDetailDto>>> GetParticipantEnrolmentsAsync(int userId) => GetAsync<List<EnrolmentDetailDto>>($"api/participants/{userId}/enrolments");
    public Task<ApiResult<object>> EnrolAsync(int userId, int eventId, int categoryId) => SendAsync<object>(HttpMethod.Post, "api/enrolments", new { participantId = userId, eventId, categoryId });
    public Task<ApiResult<object>> CancelEnrolmentAsync(int enrolmentId) => SendAsync<object>(HttpMethod.Put, $"api/enrolments/{enrolmentId}/cancel");

    public Task<ApiResult<ParticipantDto>> GetParticipantAsync(int id) => GetAsync<ParticipantDto>($"api/participants/{id}");
    public Task<ApiResult<object>> UpdateParticipantAsync(int id, ParticipantDto value) => SendAsync<object>(HttpMethod.Put, $"api/participants/{id}", value);
    public Task<ApiResult<OrganiserDto>> GetOrganiserAsync(int id) => GetAsync<OrganiserDto>($"api/organisers/{id}");

    public Task<ApiResult<object>> CreateResultAsync(int enrolmentId, TimeSpan time, int position, int totalFinishers) =>
        SendAsync<object>(HttpMethod.Post, "api/results", new { enrolmentId, finishTime = time, finishPosition = position, totalFinishers });
    public Task<ApiResult<object>> UpdateResultAsync(int resultId, TimeSpan time, int position, int totalFinishers) =>
        SendAsync<object>(HttpMethod.Put, $"api/results/{resultId}", new { finishTime = time, finishPosition = position, totalFinishers });

    public Task<ApiResult<object>> UploadBannerAsync(int eventId, IFormFile file) =>
        UploadAsync($"api/events/{eventId}/banner", file, "file");
    public Task<ApiResult<object>> UploadProfilePictureAsync(int userId, IFormFile file) =>
        UploadAsync($"api/participants/{userId}/picture", file, "file");

    private Task<ApiResult<T>> GetAsync<T>(string uri) => SendAsync<T>(HttpMethod.Get, uri);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string uri, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, uri);
            if (body != null) request.Content = JsonContent.Create(body);
            using var response = await _http.SendAsync(request);
            return await ReadResultAsync<T>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            return ApiResult<T>.Fail(0, "We could not reach the RaceDay service. Please try again shortly.");
        }
    }

    private async Task<ApiResult<object>> UploadAsync(string uri, IFormFile file, string fieldName)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, fieldName, file.FileName);
        try
        {
            using var response = await _http.PostAsync(uri, content);
            return await ReadResultAsync<object>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            return ApiResult<object>.Fail(0, "We could not reach the RaceDay service. Please try again shortly.");
        }
    }

    // Never show raw API/server text to the user.
    private static async Task<string> FriendlyErrorAsync(HttpResponseMessage response)
    {
        switch ((int)response.StatusCode)
        {
            case 401: return "Your session has expired. Please log in again.";
            case 403: return "You do not have permission to do that.";
            case 404: return "We could not find what you were looking for.";
            case >= 500: return "Something went wrong on our side. Please try again.";
        }

        var body = (await response.Content.ReadAsStringAsync()).Trim();
        if (body.Length == 0 || body.StartsWith('{') || body.StartsWith('<'))
            return "Please check the details you entered and try again.";
        return body.Trim('"');
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return ApiResult<T>.Fail((int)response.StatusCode, await FriendlyErrorAsync(response));

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            return ApiResult<T>.Ok(default, (int)response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<T>();
        return ApiResult<T>.Ok(data, (int)response.StatusCode);
    }
}