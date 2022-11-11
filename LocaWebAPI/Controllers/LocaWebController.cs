using LocaWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace LocaWebAPI.Controllers
{
    public class LocaWebController : ApiController
    {

        /// <summary>
        /// Métodos genéricos - GET.
        /// </summary>
        /// <param name="getUri"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Teste/GetLocWeb")]
        public HttpResponseMessage GetLocWeb(Uri getUri)
        {
            System.Net.HttpWebRequest httpRequest = null;
            System.Net.HttpWebResponse httpResponse = null;
            HttpResponseMessage Resposta = new HttpResponseMessage();
            string responseText;
            try
            {
                Console.Write(@"Requisitando a URL \n");
                Console.Write(@"Criando o pedido HTTP \n");
                httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(getUri.AbsoluteUri);

                Console.Write(@"Inserindo Headers \n");
                httpRequest.ContentType = "application/json ";
                string token_autorization = "JPqyNMEgzjWxq3BfPzWnscsekwp2sfsp71BrGmpjvYxr ";
                if (token_autorization != string.Empty)
                    httpRequest.Headers.Add("X-Auth-Token", string.Format("{0} {1}", "", token_autorization));

                Console.Write(@"Fazendo a solicitação.\n");
                httpResponse = (System.Net.HttpWebResponse)httpRequest.GetResponse();
                httpResponse.GetResponseStream();
                using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    Resposta.Content = new StringContent(reader.ReadToEnd());
                }

            }
            catch (Exception ex)
            {
                Resposta.Content = new StringContent("Ocorreu um erro ao realizar o GET");
                return Resposta;
            }
            return Resposta;
        }


        /// <summary>
        /// Métodos genéricos - POST
        /// </summary>
        /// <param name="PostUri"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("EnviaData")]
        public HttpResponseMessage PostLocWeb(Uri PostUri, object postData)

        {
            System.Net.WebRequest WebRequest;
            System.Net.WebResponse WebResponse;
            HttpResponseMessage httpResponseMe = new HttpResponseMessage();
            Stream httpPostStream;

            try
            {
                postData = JsonConvert.SerializeObject(postData, Formatting.Indented);
                string postString = postData.ToString();
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
                WebRequest = (HttpWebRequest)WebRequest.Create(PostUri);
                string token_autorization = "JPqyNMEgzjWxq3BfPzWnscsekwp2sfsp71BrGmpjvYxr ";
                if (token_autorization != string.Empty)
                    WebRequest.Headers.Add("X-Auth-Token", string.Format("{0} {1}", "", token_autorization));
                WebRequest.ContentType = "application/json";
                WebRequest.ContentLength = byteArray.Length;
                WebRequest.Method = "POST";
                httpPostStream = WebRequest.GetRequestStream();
                httpPostStream.Write(byteArray, 0, byteArray.Length);

                httpPostStream.Close();
                //httpPostStream = WebResponse.GetResponseStream();
                WebResponse = WebRequest.GetResponse();

                using (httpPostStream = WebResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(httpPostStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    var json = JsonConvert.SerializeObject(responseFromServer);
                    httpResponseMe.Content = new StringContent(responseFromServer);

                    return httpResponseMe;
                }
            }
            catch (WebException wex)
            {
                httpResponseMe.StatusCode = HttpStatusCode.BadRequest;
                httpResponseMe.Content = new StringContent(wex.Message);
                return httpResponseMe;
            }
        }
        /// <summary>
        /// Método que chama o método de criar a lista e adicionar diversos usuários em uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("CriaListaAdicionaUsuarioEnvia")]
        public HttpResponseMessage CriaListaAdicionaUsuarioEnvia([FromBody]  LocaWebModel.Raiz obj)
        {
            LocaWebModel.Raiz objLW = new LocaWebModel.Raiz();
            objLW = obj;
            HttpResponseMessage resposta = new HttpResponseMessage();
            HttpResponseMessage resposta2 = new HttpResponseMessage();
            HttpResponseMessage resposta3 = new HttpResponseMessage();
            string result;
            string result2;
            string result3;

            LocaWebModel.List objContato = new LocaWebModel.List();
            LocaWebModel.Lista objLista = new LocaWebModel.Lista();
            LocaWebModel.Envio objMsg = new LocaWebModel.Envio();


            objContato.contacts = obj.list.contacts;
            objLista.name = obj.list.name;
            objLista.description = obj.list.description;


            //for (int qt = 0; qt < obj.list.contacts.Count; qt++)
            //{
            //    objContato.contacts[qt].custom_fields = obj.list.contacts[qt].custom_fields;
            //    objContato.contacts[qt].email = obj.list.contacts[qt].email;

            //}

            result = CriarLista(obj.list);

            dynamic myObject = JsonConvert.DeserializeObject<dynamic>(result);
            dynamic Message = JObject.Parse(result)["id"];
            objMsg.message = obj.message;
            objMsg.message.list_ids = new List<string>();
            objMsg.message.list_ids.Add(Message.ToString());



            result2 = CriarContatosEmLista(obj.list, objMsg.message.list_ids[0]);

            if (result2 == "" || result == " ") {
                result2 = "Resultado em branco";
            }


            resposta3 = CriaMensagem(objMsg);

            result3 = resposta3.Content.ReadAsStringAsync().Result;

            result = "Primeiro Resultado" + result + " Segundo Resultado : " + result2 + "Terceiro Resultado: " + result3;

            resposta.Content = new StringContent(result);
            resposta.StatusCode = HttpStatusCode.Created;

            return resposta;

           
        }
        /// <summary>
        /// Métodos Específicos - Criar Lista
        /// </summary>
        /// <param name="obj"></param>
        /// 
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Teste/CriarLista")]
        public string CriarLista([FromBody] LocaWebModel.List obj)
        {
            HttpResponseMessage resposta;
            string result;
            string listid = "0";

            Uri _uri = new Uri("https://emailmarketing.locaweb.com.br/api/v1/accounts/177705/lists");

            resposta = PostLocWeb(_uri, obj);

            if (resposta.StatusCode == HttpStatusCode.BadRequest)
            {
                

                obj.name = obj.name + " " + DateTime.Now.ToString() ;

                resposta = PostLocWeb(_uri, obj);
                if (resposta.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception("A lista não pode ser adicionada.");
                    return "error";
                }

            }

            result = resposta.Content.ReadAsStringAsync().Result;



            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Teste/CriarContatosEmLista")]
        public string CriarContatosEmLista([FromBody] LocaWebModel.List obj, [FromUri] string listid = "0")
        {
            

            LWListasContatos.ListasContatos objLWLC = new LWListasContatos.ListasContatos();
            
            objLWLC.list = new LWListasContatos.List();
            objLWLC.list.contacts = new List<LWListasContatos.Contact>();
            

            HttpResponseMessage resposta = new HttpResponseMessage();
            string StringResposta = "";
            string result;

            if (listid == "0" || listid == "")
            {
                resposta = GetLista();
                result = resposta.Content.ToString();
                StringResposta = result;
                dynamic myObject = JsonConvert.DeserializeObject<dynamic>(StringResposta);
                dynamic Message = JObject.Parse(StringResposta)["items"].First["id"];
            }


            List<LWListasContatos.Contact> objListaErro = new List<LWListasContatos.Contact>();

            var _lst_contatos = new LWListasContatos.Contact();

            for (int i3 = 0; i3 < obj.contacts.Count; i3++) {
                if (obj.contacts[i3].email.Length <= 1)
                {
                    obj.contacts[i3].email = obj.contacts[i3].email + " Email Inválido";
                }
            }

            var _contatosSimples = obj.contacts.Select(c => new { email = c.email.ToLower(), custom_fields = c.custom_fields }).ToList();
            var _contatos_distintos = _contatosSimples.GroupBy(f => f.email)
                                        .Select(g => new { email = g.Key, custom_fields = (_contatosSimples.Select(a => a.custom_fields).
                                                                            Where(r=>r.name == g.Min(m => m.custom_fields.name)).First()) }).ToList();
            obj.contacts = _contatos_distintos.Select(c => new LocaWebModel.Contact
            {
                email = c.email,
                custom_fields = c.custom_fields
            }).ToList();

            foreach (var _contatos in obj.contacts)
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(_contatos.email);
                if (match.Success)
                {

                    objLWLC.list.contacts.Add(new LWListasContatos.Contact
                    {
                        email = _contatos.email,
                        custom_fields = new LWListasContatos.CustomFields
                        {
                            name = _contatos.custom_fields.name
                        }
                    });

                }
                else
                    objListaErro.Add(new LWListasContatos.Contact
                    {
                        email = _contatos.email
                    });
                }
            
        
    



            objLWLC.list.overwriteattributes = obj.overwriteattributes;

            Uri _uri = new Uri("https://emailmarketing.locaweb.com.br/api/v1/accounts/177705/lists/"+ listid + "/contacts");
            resposta = PostLocWeb(_uri, objLWLC);

            if (resposta.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception("Contatos não foram adicionados.");
                return "error";
            }

                result = resposta.Content.ReadAsStringAsync().Result;

            return result;
        }

        /// <summary>
        /// Método para criar contato
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Teste/CriaMensagem")]
        public HttpResponseMessage CriaMensagem([FromBody]LocaWebModel.Envio obj)
        {
            if (obj.message.scheduled_to == null || obj.message.scheduled_to ==  "")
            {
                obj.message.scheduled_to = DateTime.Now.ToString();
            }
            HttpResponseMessage resposta = null;
            string result = null;

            Uri _uri = new Uri("https://emailmarketing.locaweb.com.br/api/v1/accounts/177705/messages");
            resposta = PostLocWeb(_uri, obj);
            result = resposta.Content.ReadAsStringAsync().Result;



            return resposta;
        }


        /// <summary>
        /// Métodos Específicos - Criar contato
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Teste/CriarContato")]
        public HttpResponseMessage PostContato([FromBody]LocaWebModel.Contact obj)
        {
            HttpResponseMessage resposta;
            string result = null;
            Uri _uri = new Uri("https://emailmarketing.locaweb.com.br/api/v1/accounts/177705/contacts");
            resposta = PostLocWeb(_uri, obj);
            result = resposta.Content.ReadAsStringAsync().Result;
            return resposta;
        }
        /// <summary>
        ///Métodos Específicos - listar listas
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Teste/GetLista")]
        public HttpResponseMessage GetLista()
        {
         HttpResponseMessage result;
         Uri _uri = new Uri("https://emailmarketing.locaweb.com.br/api/v1/accounts/177705/lists");
         result = GetLocWeb(_uri);
         return result;
        }

      
    }

}
