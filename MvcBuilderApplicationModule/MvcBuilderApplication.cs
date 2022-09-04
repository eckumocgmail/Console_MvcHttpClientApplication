using MvcApplicationBuilderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace MvcApplicationBuilderModule
{
    public class NamedObject
    {
        public virtual string Name { get; set; }    
    }
        
    public class BusinessFunction  
    {

    }
    public class MessageProperty 
    {

    }
    public class BusinessResource  
    {

    }
    public class MessageProtocol  
    {

    }
    public interface IBuilderCovariant<out TResult>
    {
        public TResult Build();
    }
    public interface ICollectionBuilder<TItem>
    {
        public bool Add(TItem val);
        public bool Remove(TItem val);
    }

    public interface IDictionaryBuilder<TValue>
    {
        public TValue Get(string key);
        public void Set(string key, TValue value);
        public void Clear();
        public IEnumerable<string> GetKeys();
        public bool RemoveByKey(string key);
    }



   


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcParameterBuilder : IBuilderCovariant<MyParameterDeclarationModel>
    {
        public void SetType(string type);
        public string GetType();
        public string GetName();
        public string SetName(string value);

        public IEnumerable<string> GetAttributeNames();
        public string GetAttribute(string name, string value);
        public string SetAttribute(string name, string value);
    }


 

    /// <summary>
    /// 
    /// </summary>
    public interface IMvcActionBuilder : IBuilderCovariant<MyActionModel>
    {
        public IMvcParameterBuilder GetParameter(string name);
        public IMvcParameterBuilder CreateParameter(string name);
        public IMvcParameterBuilder RemoveParameter(string name);
        
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcActionApplicationBuilder
    {
        public IMvcParameterBuilder GetParameter(string area, string controller, string action, string name);
        public IMvcParameterBuilder CreateParameter(string area, string controller, string action, string name);
        public IMvcParameterBuilder RemoveParameter(string area, string controller, string action, string name);
    }


 

    /// <summary>
    /// 
    /// </summary>
    public interface IMvcControllerBuilder : IBuilderCovariant<MyControllerModel>
    {

        public IMvcActionBuilder GetControllerAction(string name);
        public IMvcActionBuilder CreateControllerAction(string name);
        public IMvcActionBuilder RemoveControllerAction(string name);
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcControllerApplicationBuilder
    {

        public IEnumerable<IMvcActionBuilder> GetControllerActions(string area );
        public IMvcActionBuilder GetControllerAction(string area, string name);
        public IMvcActionBuilder CreateControllerAction(string area, string name);
        public IMvcActionBuilder RemoveControllerAction(string area, string name);
    }

    


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcAreaBuilder : IBuilderCovariant<AppAreaModel>
    {
        public IEnumerable<IMvcControllerBuilder> GetControllers( );
        public IMvcControllerBuilder GetController(string name);
        public IMvcControllerBuilder CreateController(string name);
        public IMvcControllerBuilder RemoveController(string name);
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcAreaApplicationBuilder : IMvcAreaBuilder
    {
        public IEnumerable<IMvcControllerBuilder> GetControllers(string area);

        public IMvcControllerBuilder GetController(string area, string name);
        public IMvcControllerBuilder CreateController(string area, string name);
        public IMvcControllerBuilder RemoveController(string area, string name);
    }



    /// <summary>
    /// 
    /// </summary>
    public interface IMessagePropertyBuilder
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMessageProtocolBuilder : IDictionaryBuilder<MessageProperty>, IBuilderCovariant<MessageProtocol>
    {
        public IMessagePropertyBuilder GetMessageProperty(string name);
        public IMessagePropertyBuilder CreateMessageProperty(string name);
        public IMessagePropertyBuilder RemoveMessageProperty(string name);
    }


    public interface IMessageProtocolApplicationBuilder
    {
        public IMessagePropertyBuilder GetMessageProperty(string resource, string function, string name);
        public IMessagePropertyBuilder CreateMessageProperty(string resource, string function, string name);
        public IMessagePropertyBuilder RemoveMessageProperty(string resource, string function, string name);

    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessFunctionBuilder : IBuilderCovariant<BusinessFunction>
    {

        public IMessageProtocolBuilder GetMessageProtocol(string name);
        public IMessageProtocolBuilder CreateMessageProtocol(string name);
        public IMessageProtocolBuilder RemoveMessageProtocol(string name);
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessFunctionApplicationBuilder
    {
        public IMessageProtocolBuilder GetMessageProtocol(string function, string name);
        public IMessageProtocolBuilder CreateMessageProtocol(string function, string name);
        public IMessageProtocolBuilder RemoveMessageProtocol(string function, string name);
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessResourceBuilder : IBuilderCovariant<BusinessResource>
    {
        public IBusinessFunctionBuilder GetBusinessFunction(string name);
        public IBusinessFunctionBuilder CreateBusinessFunction(string name);
        public IBusinessFunctionBuilder RemoveBusinessFunction(string name);

    }


    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessResourceApplicationBuilder
    {
        public IBusinessFunctionBuilder GetBusinessFunction(string businessResource, string name);
        public IBusinessFunctionBuilder CreateBusinessFunction(string businessResource, string name);
        public IBusinessFunctionBuilder RemoveBusinessFunction(string businessResource, string name);

    }


    public class MvcApplicationModel : NamedObject
    {

    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMvcApplicationBuilder : IBuilderCovariant<MvcApplicationModel>
    {
        public IMvcAreaBuilder GetArea(string name);
        public IMvcAreaBuilder CreateArea(string name);

        public IBusinessResourceBuilder GetBusinessResource(string name);
        public IBusinessResourceBuilder CreateBusinessResource(string name);

    }


    /// <summary>
    /// 
    /// </summary>
    public class MvcApplicationBuilder : IMvcApplicationBuilder
    {
        public ConcurrentDictionary<string, IMvcAreaBuilder> Areas = new ConcurrentDictionary<string, IMvcAreaBuilder>();
        public ConcurrentDictionary<string, IBusinessResourceBuilder> Resources = new ConcurrentDictionary<string, IBusinessResourceBuilder>();

        public string Directory { get; set; }

        public MvcApplicationBuilder()
        {
     
            this.Directory = SaveFileDialog();
            if (System.IO.Directory.Exists(Directory) == false)                
                System.IO.Directory.CreateDirectory(Directory);
                
               
        }

        public string SaveFileDialog() => CmdExec("Win_SaveFIleDialogApplication.exe");


        /// <summary>
        /// Выполнение инструкции через командную строку
        /// </summary>
        /// <param name="command"> команда </param>
        /// <returns></returns>
        [Display(Name = "Выполнение инструкции через командную строку")]
        public string CmdExec(string command)
        {
            command = command.ReplaceAll("\n", "").ReplaceAll("\r", "").ReplaceAll(@"\\", @"\").ReplaceAll(@"//", @"/");

            Console.WriteLine(command);


            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C " + command);

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
            string response = process.StandardOutput.ReadToEnd();
            string result = response.ReplaceAll("\r", "\n");
            result = result.ReplaceAll("\n\n", "\n");
            while (result.EndsWith("\n"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public MvcApplicationBuilder(string path=null)
        {
            if (!System.IO.Directory.Exists(path))
            {
                CreateDirectoryPath(path);
            }
        }



        public static void CreateDirectoryPath(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        [Display(Name = "Отладить сегмент связи")]
        public IMvcAreaBuilder GetArea(string name)
        {
            if (Areas.ContainsKey(name) == false)
                Areas[name] = this.CreateArea(name);
            return Areas[name];
        }

        [Display(Name = "НАстроить интерфейс управления связью")]
        public IBusinessResourceBuilder GetBusinessResource(string name)
        {
            if (Resources.ContainsKey(name) == false)
                Resources[name] = this.CreateBusinessResource(name);
            return Resources[name];
        }


        [Display(Name = "Зарегистрировать новый сегмент связи")]
        public IMvcAreaBuilder CreateArea(string name) => new MvcAreaBuilder(this, name);

        public IBusinessResourceBuilder CreateBusinessResource(string name) => new BusinessResourceBuilder(this, name);


        [Display(Name = "Собрать приложение")]
        public MvcApplicationModel Build()
        {
            throw new System.NotImplementedException();
        }

        public class MvcAreaBuilder : NamedObject, IMvcAreaBuilder
        {
            public MvcAreaBuilder(MvcApplicationBuilder mvcApplicationBuilder, string name)
            {
                MvcApplicationBuilder = mvcApplicationBuilder;
            }

            public MvcApplicationBuilder MvcApplicationBuilder { get; }


            public IMvcControllerBuilder GetController(string name)
            {
                throw new System.NotImplementedException();
            }

            public IMvcControllerBuilder CreateController(string name) => new MvcControllerBuilder(this, name);

            public IMvcControllerBuilder RemoveController(string name)
            {
                throw new System.NotImplementedException();
            }

            public AppAreaModel Build()
            {
                throw new System.NotImplementedException();
            }

            public class MvcControllerBuilder : NamedObject, IMvcControllerBuilder
            {
                public MvcControllerBuilder(MvcAreaBuilder mvcAreaBuilder, string name)
                {
                    MvcAreaBuilder = mvcAreaBuilder;
                    Name = name;
                }

                public MvcAreaBuilder MvcAreaBuilder { get; }
                public string Name { get; }

                public IMvcActionBuilder GetControllerAction(string name)
                {
                    throw new System.NotImplementedException();
                }

                public IMvcActionBuilder CreateControllerAction(string name) => new MvcActionBuilder(this, name);

                public IMvcActionBuilder RemoveControllerAction(string name)
                {
                    throw new System.NotImplementedException();
                }

                public AppControllerModel Build()
                {
                    throw new System.NotImplementedException();
                }

                /// <summary>
                /// 
                /// </summary>
                public class MvcActionBuilder : NamedObject, IMvcActionBuilder
                {

                    public MvcControllerBuilder MvcControllerBuilder { get; }

                    public MvcActionBuilder(MvcControllerBuilder mvcControllerBuilder, string name)
                    {
                        MvcControllerBuilder = mvcControllerBuilder;
                        Name = name;
                    }

                  

                    public IMvcParameterBuilder GetParameter(string name)
                    {
                        throw new System.NotImplementedException();
                    }

                    public IMvcParameterBuilder CreateParameter(string name) => new MvcParameterBuilder(this, name);

                    public IMvcParameterBuilder RemoveParameter(string name)
                    {
                        throw new System.NotImplementedException();
                    }

                    public AppActionModel Build()
                    {
                        throw new System.NotImplementedException();
                    }

                    public class MvcParameterBuilder : NamedObject, IMvcParameterBuilder
                    {
                        public MvcActionBuilder MvcActionBuilder { get; }


                        public MvcParameterBuilder()
                        {
                        }

                        public MvcParameterBuilder(MvcActionBuilder mvcActionBuilder, string name)
                        {
                            MvcActionBuilder = mvcActionBuilder;
                            Name = name;
                        }

                        public string Type { get; set; }
                        public void SetType(string type) => this.Type = type;
                        string IMvcParameterBuilder.GetType() => this.Type;

                        public string GetName() => this.Name;

                      

                        public IEnumerable<string> GetAttributeNames()
                        {
                            throw new System.NotImplementedException();
                        }

                        public string GetAttribute(string name, string value)
                        {
                            throw new System.NotImplementedException();
                        }

                        public string SetAttribute(string name, string value)
                        {
                            throw new System.NotImplementedException();
                        }

                        public AppParameterModel Build()
                        {
                            throw new System.NotImplementedException();
                        }

                        public string SetName(string value)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }

            public IEnumerable<IMvcControllerBuilder> GetControllers()
            {
                throw new NotImplementedException();
            }
        }

        public class BusinessResourceBuilder : NamedObject, IBusinessResourceBuilder
        {
            public BusinessResourceBuilder(MvcApplicationBuilder mvcApplicationBuilder, string name)
            {
                MvcApplicationBuilder = mvcApplicationBuilder;
                Name = name;
            }

            public MvcApplicationBuilder MvcApplicationBuilder { get; }

            public IBusinessFunctionBuilder GetBusinessFunction(string name)
            {
                throw new System.NotImplementedException();
            }

            public IBusinessFunctionBuilder CreateBusinessFunction(string name) => new BusinessFunctionBuilder(this, name);

            public IBusinessFunctionBuilder RemoveBusinessFunction(string name)
            {
                throw new System.NotImplementedException();
            }

            public BusinessResource Build()
            {
                throw new System.NotImplementedException();
            }

            public class BusinessFunctionBuilder : NamedObject, IBusinessFunctionBuilder
            {
                public BusinessFunctionBuilder(BusinessResourceBuilder businessResourceBuilder, string name)
                {
                    BusinessResourceBuilder = businessResourceBuilder;
                    Name = name;
                }

                public BusinessResourceBuilder BusinessResourceBuilder { get; }


                public IMessageProtocolBuilder GetMessageProtocol(string name)
                {
                    throw new System.NotImplementedException();
                }

                public IMessageProtocolBuilder CreateMessageProtocol(string name) => new MessageProtocolBuilder(this, name);

                public IMessageProtocolBuilder RemoveMessageProtocol(string name)
                {
                    throw new System.NotImplementedException();
                }

                public BusinessFunction Build()
                {
                    throw new System.NotImplementedException();
                }

                public class MessageProtocolBuilder : NamedObject, IMessageProtocolBuilder
                {
                    public MessageProtocolBuilder(BusinessFunctionBuilder businessFunctionBuilder, string name)
                    {
                        BusinessFunctionBuilder = businessFunctionBuilder;
                        Name = name;
                    }

                    public BusinessFunctionBuilder BusinessFunctionBuilder { get; }

                    public IMessagePropertyBuilder GetMessageProperty(string name)
                    {
                        throw new System.NotImplementedException();
                    }

                    public IMessagePropertyBuilder CreateMessageProperty(string name) => new MessagePropertyBuilder(this, name);

                    public IMessagePropertyBuilder RemoveMessageProperty(string name)
                    {
                        throw new System.NotImplementedException();
                    }

                    MessageProperty IDictionaryBuilder<MessageProperty>.Get(string key)
                    {
                        throw new System.NotImplementedException();
                    }

                    public void Set(string key, MessageProperty value)
                    {
                        throw new System.NotImplementedException();
                    }

                    public void Clear()
                    {
                        throw new System.NotImplementedException();
                    }

                    public IEnumerable<string> GetKeys()
                    {
                        throw new System.NotImplementedException();
                    }

                    public bool RemoveByKey(string key)
                    {
                        throw new System.NotImplementedException();
                    }

                    public MessageProtocol Build()
                    {
                        throw new System.NotImplementedException();
                    }

                    public class MessagePropertyBuilder : NamedObject, IMessagePropertyBuilder
                    {
                        public MessagePropertyBuilder(MessageProtocolBuilder messageProtocolBuilder, string name)
                        {
                            MessageProtocolBuilder = messageProtocolBuilder;
                            Name = name;
                        }

                        public MessageProtocolBuilder MessageProtocolBuilder { get; }

                    }
                }
            }
        }
    }


    public static class Extensions
    {
        public static string ReplaceAll(this string text, string s1, string s2)
        {
            while(text.IndexOf(s1)!=-1)
                text = text.Replace(s1, s2);
            return text;
        }
    }

}
