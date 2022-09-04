using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

/// <summary>
/// Клиент контроллера помеченного атрибутов [Route("[controller]/[action]")]
/// </summary>
public class HttpClientController : Controller
{
    protected string _url;
    protected bool _logging = true;

    protected ITokenStorage _storage;
    protected HttpClient _http;

    protected IDictionary<string, string> _cookies =
        new Dictionary<string, string>();


    /// <summary>
    /// Конструктор http-клиента настроенного на работу с контроллером
    /// </summary>
    /// <param name="url">URL адрес контроллера помеченного атрибутов [Route("[controller]/[action]")]</param>
    public HttpClientController( )
    {
        _storage = new FileTokenStorage($"{GetType().Name}.cookies.txt");
        _http = new HttpClient();
    }


    public async Task<string> GetToken()
    {
        await Task.CompletedTask;
        return _storage.Get();

    }
    public async Task SetToken(string value)
    {
        _storage.Set(value);
        await Task.CompletedTask;
    }


    /// <summary>
    /// Выполнение запроса методом POST
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения, передаваемого в теле запроса</typeparam>
    /// <param name="action">имя операции</param>
    /// <param name="Message">данные для запроса</param>
    /// <returns>текст тела сообщения</returns>
    public async Task<string> Post<TMessage, TResult>(string action, TMessage Message)
    {
        string url = _url + "/" + action;
        if (_logging) this.Info($"[POST][{url}]");
        string token = await GetToken();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("HTTP", token);
        string RequestBody = JObject.FromObject(Message).ToString();
        request.Content = new StringContent(RequestBody, Encoding.UTF8, "text/plain");
        HttpResponseMessage httpResponseMessage = await _http.SendAsync(request);
        return await LogHttpResponse(url, httpResponseMessage);
    }

    private void Info(string message)
        => Console.WriteLine($"[{GetType().Name}][INFO]: {message}");
        
    

    /// <summary>
    /// Выполнение запроса методом POST
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения, передаваемого в теле запроса</typeparam>
    /// <param name="action">имя операции</param>
    /// <param name="Headers">заголовки запроса</param>
    /// <param name="Message">данные для запроса</param>
    /// <returns>текст тела сообщения</returns>
    public async Task<TMessage> Post<TMessage>(string action,
    IDictionary<string, string> Headers,
    IDictionary<string, string> Parameters)
    {
        string url = _url + "/" + action;
        if (_logging) this.Info($"[POST][{url}]");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        bool firstParameter = true;
        foreach (var kv in Parameters)
        {
            if (firstParameter)
            {
                url += $"?{kv.Key}={kv.Value}";
                firstParameter = false;
            }
            else
            {
                url += $"&{kv.Key}={kv.Value}";
            }
        }
        //string token = await _storage.Get();
        //request.Headers.Authorization = 
        //    new System.Net.Http.Headers.AuthenticationHeaderValue("HTTP", token);
        //request.Headers.Add("Cookie", "UserKey=" + token);
        string RequestBody = JObject.FromObject(Parameters).ToString();
        request.Content = new StringContent(RequestBody, Encoding.UTF8, "text/plain");
        HttpResponseMessage httpResponseMessage = await _http.SendAsync(request);
        string responseText = await LogHttpResponse(url, httpResponseMessage);
        if (typeof(TMessage).IsPrimitive)
        {
            return (TMessage)Set(responseText, typeof(TMessage).Name);
        }
        else
        {
            return FromJson<TMessage>(responseText);
        }
    }

    private TMessage FromJson<TMessage>(string responseText)
    {
        try
        {
            return JsonConvert.DeserializeObject<TMessage>(responseText);
        }
        catch (Exception ex)
        {
            throw new Exception("Исключение при десериализации: " + responseText, ex);
        }
    }

    private object Set(string responseText, string name)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Выполнение запроса методом GET
    /// </summary>
    /// <param name="action">имя операции</param>
    /// <returns>текст из тела ответного сообщения</returns>
    /*public async Task<string> Get(string action)
    {
        string url = _url + "/" + action;
        if (_logging) this.Info($"[GET][{url}]");

        var response = await _http.GetAsync(url);
        string Body = await LogHttpResponse (url, response);
        return Body;
    }*/

    /// <summary>
    /// Выполнение запроса методом GET
    /// </summary>
    /// <param name="action">имя операции</param>
    /// <returns>текст из тела ответного сообщения</returns>
    public async Task<TResponse> Get<TResponse>(string action)
    {
        string token = _storage.Get();
        string key = token;
        var headers = new Dictionary<string, string>()
            {
                { "Authorization",key },
                { "Cookie","UserKey="+key },
            };

        return await Get<TResponse>(action, headers);
    }
    public async Task<TResponse> Get<TResponse>(string action, Dictionary<string, string> Headers)
    {
        string url = _url + "/" + action;

        if (_logging) this.Info($"[POST][{url}]");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var kv in Headers)
        {

            request.Headers.Add(kv.Key, kv.Value);
        }
        string token = _storage.Get();
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("HTTP", token);
        request.Headers.Add("Cookie", "UserKey=" + token);

        request.Content = new StringContent(null, Encoding.UTF8, "text/plain");
        HttpResponseMessage httpResponseMessage = await _http.SendAsync(request);
        string body = await LogHttpResponse(url, httpResponseMessage);
        return FromJson<TResponse>(body);

        /*var response = await _http.GetAsync(url);
        string Body = await LogHttpResponse(url, response);

        return Body.Deseriallize<TResponse>();*/
    }

    public async Task<TResponse> Get<TResponse>(string action,

        IDictionary<string, string> parameters)
    {
        string url = _url + "/" + action;
        bool firstParameter = true;
        foreach (var kv in parameters)
        {
            if (firstParameter)
            {
                url += $"?{kv.Key}={kv.Value}";
                firstParameter = false;
            }
            else
            {
                url += $"&{kv.Key}={kv.Value}";
            }
        }


        if (_logging) this.Info($"[GET][{url}]");
        _http.DefaultRequestHeaders.Add("Cookie", "");
        var response = await _http.GetAsync(url);
        string responseText = await LogHttpResponse(url, response);

        if (typeof(TResponse).IsPrimitive)
        {
            return (TResponse)Set(responseText, typeof(TResponse).Name);
        }
        else
        {
            return FromJson<TResponse>(responseText);
        }

        /*
        string url = _url + "/" + action;
        bool firstParameter = true;
        foreach (var kv in parameters)
        {
            if (firstParameter)
            {
                url += $"?{kv.Key}={kv.Value}";
                firstParameter = false;
            }
            else
            {
                url += $"&{kv.Key}={kv.Value}";
            }
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        string token = await GetToken();
        if (string.IsNullOrEmpty(token) == false)
        {
            Headers["Authorization"] = token;
            request.Headers.Add("Cookie", "UserKey=" + token);
        }

        //request.Headers.Authorization =
            //new System.Net.Http.Headers.AuthenticationHeaderValue("HTTP", token);            
        //request.Content = new StringContent(new { }.ToJson(), Encoding.UTF8, "text/plain");
        HttpResponseMessage httpResponseMessage = await _http.SendAsync(request);
        string responseText = await LogHttpResponse(action, httpResponseMessage);
        */


    }


    /// <summary>
    /// Логированние ответного сообщения включая заголовки
    /// </summary>
    /// <param name="action">Имя операции</param>
    /// <param name="response">Ссылка на сообщение</param>
    /// <returns>текст тела сообщения</returns>
    private async Task<string> LogHttpResponse(string action, HttpResponseMessage response)
    {
        if (_logging) this.Info($"[GET][{action}][{response.StatusCode}]");
        var enumerator = response.Headers.GetEnumerator();
        if (_logging) this.Info($"Headers:");
        while (enumerator.MoveNext())
        {
            if (_logging)
            {
                string values = "[";
                enumerator.Current.Value.ToList().ForEach(value =>
                {
                    values += value + ",";
                });
                values = values.Substring(0, values.Length - 1) + "]";
                this.Info($" \t {enumerator.Current.Key}={values}");
            }
            if (enumerator.Current.Key == "Set-Cookie")
            {
                var values = enumerator.Current.Value.ToArray();
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];


                    var model = new CookieModel();
                    IDictionary<string, string> parameters = ParseParameters(value.ToString());
                    if (parameters.ContainsKey("UserKey") == false)
                    {
                        throw new Exception("Не удалось получить параметр UserKey");
                    }
                    else
                    {
                        model.UserKey = parameters["UserKey"];
                    }
                    if (parameters.ContainsKey("path") == false)
                    {
                        throw new Exception("Не удалось получить параметр UserKey");
                    }
                    else
                    {
                        model.Path = parameters["path"];
                    }


                    this._cookies[model.Path] = model.UserKey;

                    string token = model.UserKey;
                    _storage.Set(token);
                    this.Info("Сохраняем куки в локальное хранилище " + model.UserKey);
                };
            }
        }
        string Body = await response.Content.ReadAsStringAsync();
        if (_logging) this.Info($" Body:");
        if (_logging) this.Info($"{Body}");
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.NoContent:
                return "";
            case System.Net.HttpStatusCode.OK:
                return Body;

            case System.Net.HttpStatusCode.InternalServerError:
                throw new Exception(Body);
            default:
                throw new NotImplementedException("Обработка статуса " + response.StatusCode.ToString() + " в настоящий момент не выполняется");
        }

    }

    private IDictionary<string, string> ParseParameters(string message)
    {
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        foreach (string text in message.Split(";"))
        {
            string[] items = text.Trim().Split("=");
            if (items.Length != 2)
            {
                throw new Exception("Не удалось разобрать текст как параметры");
            }
            parameters[items[0].Trim()] = items[1].Trim();
        }
        return parameters;
    }


    private class CookieModel
    {
        public CookieModel()
        {
        }

        public string UserKey { get; set; }
        public string Path { get; set; }
    }
}