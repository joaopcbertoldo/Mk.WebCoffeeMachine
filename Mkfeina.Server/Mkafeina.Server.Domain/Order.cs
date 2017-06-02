using Mkafeina.Domain.Entities;

namespace Mkfeina.Server.Domain
{
	public enum OrderStatusEnum
	{
		Undef = 0,
		OrderWaiting,
		OrderTaken,
		BeingProcessed
	}

	public class Order
	{
		public Recipe Recipe { get; set; }

		public uint Reference { get; set; }

		public string CustomerEmail { get; set; }

		public OrderStatusEnum Status { get; set; }
	}
}