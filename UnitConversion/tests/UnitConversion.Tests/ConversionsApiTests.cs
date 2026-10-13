using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Tests;

public class ConversionsApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Convert_ReturnsResult()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/conversions/convert",
            new { category = "temperature", from = "celsius", to = "fahrenheit", value = 100 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(212, json.RootElement.GetProperty("result").GetDouble(), precision: 6);
    }

    [Fact]
    public async Task Get_Convert_ReturnsResult()
    {
        var response = await _client.GetAsync("/api/v1/conversions/convert?category=length&from=km&to=m&value=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2000, json.RootElement.GetProperty("result").GetDouble(), precision: 6);
    }

    [Fact]
    public async Task Post_Convert_UnknownUnit_Returns400ProblemDetails()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/conversions/convert",
            new { category = "length", from = "meter", to = "parsec", value = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("unknown_unit", json.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Post_Convert_MissingValue_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/conversions/convert",
            new { category = "length", from = "meter", to = "foot" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Categories_ListsUnits()
    {
        var response = await _client.GetAsync("/api/v1/conversions/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("temperature", body);
    }

    [Fact]
    public async Task Get_UnknownCategory_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/conversions/categories/color");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
