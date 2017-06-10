using System;

namespace Mkafeina.Server.Domain.Entities
{
	public enum OrderStatusEnum
	{
		Undef = 0,
		InQueue,
		Taken,
		BeingProcessed,
		Ready,
		Canceled
	}

	public class Order
	{
		public string RecipeName { get; set; }

		public string Reference { get; set; }

		public string CustomerEmail { get; set; }

		public OrderStatusEnum Status { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime TakenTime { get; set; }

		public DateTime ReadyOrCanceledTime { get; set; }
	}
}