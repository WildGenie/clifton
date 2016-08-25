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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Clifton.Core.ServiceInterfaces;

namespace Clifton.Core.Services.SemanticProcessorService
{
	public class ThreadSemaphore<T>
	{
		public int Count { get { return requests.Count; } }
		protected Semaphore sem;

		// Requests on this thread.
		protected ConcurrentQueue<T> requests;

		public ThreadSemaphore()
		{
			sem = new Semaphore(0, Int32.MaxValue);
			requests = new ConcurrentQueue<T>();
		}

		public void Enqueue(T context)
		{
			requests.Enqueue(context);
			sem.Release();
		}

		public void WaitOne()
		{
			sem.WaitOne();
		}

		public bool TryDequeue(out T context)
		{
			return requests.TryDequeue(out context);
		}
	}
}
