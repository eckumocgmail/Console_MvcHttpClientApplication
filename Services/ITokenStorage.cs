using System.ComponentModel;
using System.Threading.Tasks;

[Description("Контроллер предназначен для .")]
public interface ITokenStorage 
{
     
    //public Task<string> GetAsync();
    //public Task SetAsync(string value);
    public string Get();
    public void Set(string token);
}
