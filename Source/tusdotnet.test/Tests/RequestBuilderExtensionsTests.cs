﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Shouldly;
using tusdotnet.test.Extensions;
using Xunit;

namespace tusdotnet.test.Tests
{
	public class RequestBuilderExtensionsTests
	{
		[Fact]
		public async Task Should_Set_Tus_Resumable_Header_Properly()
		{
			using (var server = TestServer.Create(app => { }))
			{
				await server.CreateRequest("/")
					.And(r => r.Headers.Contains("Tus-Resumable").ShouldBeFalse())
					.AddTusResumableHeader()
					.And(r =>
					{
						r.Headers.Contains("Tus-Resumable").ShouldBeTrue();
						r.Headers.GetValues("Tus-Resumable").Count().ShouldBe(1);
						r.Headers.GetValues("Tus-Resumable").First().ShouldBe("1.0.0");

					})
					.GetAsync();
			}
		}

		[Theory]
		[InlineData("a", "b")]
		[InlineData("b", "a")]
		[InlineData("a", "a")]
		public async Task Should_Set_XHttpMethodOverride_Properly(string @override, string method)
		{
			using (var server = TestServer.Create(app => { }))
			{
				await server.CreateRequest("/")
					.And(r => r.Headers.Contains("X-Http-Method-Override").ShouldBeFalse())
					.OverrideHttpMethodIfNeeded(@override, method)
					.And(r =>
					{

						if (@override == method)
						{
							r.Headers.Contains("X-Http-Method-Override").ShouldBeFalse();
						}
						else
						{
							r.Headers.Contains("X-Http-Method-Override").ShouldBeTrue();
							r.Headers.GetValues("X-Http-Method-Override").Count().ShouldBe(1);
							r.Headers.GetValues("X-Http-Method-Override").First().ShouldBe(@override);
						}

					})
					.GetAsync();
			}
		}
	}
}
