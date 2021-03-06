﻿/* The MIT License (MIT)
* 
* Copyright (c) 2015 Marc Clifton
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Net;

using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;
using Clifton.Core.Workflow;

namespace Clifton.WebInterfaces
{
	public interface IWebDefaultWorkflowService : IService 
	{
		void RegisterAppTemplateObject(string name, object obj);
	}

    public interface IWebSocketServerService : IService
    {
		void Start(string ipAddress, int port, string path);
    }

	public interface IWebSocketSession
	{
		void Reply(string msg);
	}

	public interface IWebSocketClientService : IService
	{
		void Start(string ipAddress, int port, string path);
		void Send(string msg);
	}

	public interface ILoginService : IService
	{
	}

	//public interface IPublicRouterService : IService
	//{
	//	Dictionary<string, RouteInfo> Routes { get; }
	//	void RegisterSemanticRoute<T>(string path) where T : SemanticRoute;
	//}

	public interface IAuthenticatingRouterService : IService
	{
		Dictionary<string, RouteInfo> Routes { get; }
		bool IsAuthenticatedRoute(string path);
		void RegisterSemanticRoute<T>(string path, RouteType routeType = RouteType.AuthenticatedRoute, Role role = Role.None) where T : SemanticRoute;
	}

	public interface IWebSessionService : IService
	{
		void UpdateState(HttpListenerContext context);
		void Authenticate(HttpListenerContext context);
		void Logout(HttpListenerContext context);
		string GetSessionObject(HttpListenerContext context, string objectName);
		T GetSessionObject<T>(HttpListenerContext context, string objectName);
		dynamic GetSessionObjectAsDynamic(HttpListenerContext context, string objectName);
		void SetSessionObject(HttpListenerContext context, string objectName, object val);
		SessionState GetState(HttpListenerContext context);
		bool IsAuthenticated(HttpListenerContext context);
		bool IsExpired(HttpListenerContext context);
	}

	public interface IWebResponder : IService { }

	public interface IWebFileResponse : IService
	{
		bool ProcessFileRequest(HttpListenerContext context);
	}

	public interface IWebServerService : IService
	{
		List<IPAddress> GetLocalHostIPs();
		void Start(string ip, int[] ports);
	}

    public interface IWebRestService : IService
    {
		string Get(string url);
		R Get<R>(string url) where R : IRestResponse;
        R Post<R>(string url, object obj) where R : IRestResponse;
    }

    public interface IRestResponse
    {
        string RawJsonRet { get; set; }
    }

	public interface IWebWorkflowService : IService
	{
		void RegisterPreRouterWorkflow(WorkflowItem<PreRouteWorkflowData> item);
		void RegisterPostRouterWorkflow(WorkflowItem<PostRouteWorkflowData> item);
		bool PreRouter(HttpListenerContext context);
		bool PostRouter(HttpListenerContext context, HtmlResponse response);
	}
}
