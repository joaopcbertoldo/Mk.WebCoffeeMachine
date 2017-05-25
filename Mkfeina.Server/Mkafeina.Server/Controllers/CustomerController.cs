﻿using Mkafeina.Server.Domain;
using RazorEngine;
using System.Net.Http;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class CustomerController : ApiController
	{
		[HttpPost]
		[Route("api/customer/order")]
#warning evoluir para ihttpactionresult
		public CustomerOrderResponse Order([FromBody] CustomerOrderRequest request)
		{
			if (CMProxy.MachineIsRegistered(request.MachineUniqueName))
				return CMProxy.GetProxyByUniqueName(request.MachineUniqueName).HandleClientOrderRequest(request);
			else
				return new CustomerOrderResponse()
				{
					Message = "Error: inexistent coffee machine."
				};
		}

	}
}