using AutoMapper;
using SPMA.Dtos.Core;
using SPMA.Dtos.Orders;
using SPMA.Dtos.Production;
using SPMA.Dtos.Sales;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Core;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Sales;
using SPMA.Models.Warehouse;
using System.Collections.Generic;

namespace SPMA.Data
{
    public class MappingProfile : Profile
    {
        #region Constructor
        public MappingProfile()
        {
            // Map setup
            CreateMap<Order, OrderDto>().ForMember(o => o.Position, opt => opt.Ignore());
            CreateMap<OrderDto, Order>().ForSourceMember(o => o.Position, opt => opt.DoNotValidate());

            CreateMap<Component, ComponentDto>();
            CreateMap<ComponentDto, Component>();

            CreateMap<ProductionSocket, ProductionSocketDto>();
            CreateMap<ProductionSocketDto, ProductionSocket>();

            CreateMap<ProductionState, ProductionStateDto>();
            CreateMap<ProductionStateDto, ProductionState>();

            CreateMap<InProduction, InProductionDto>();
            CreateMap<InProductionDto, InProduction>();

            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();
            CreateMap<Client, ClientDto>();
            CreateMap<ClientDto, Client>();
            CreateMap<Ware, WareDto>();
            CreateMap<WareDto, Ware>();

            CreateMap<Service, ServiceDto>();
            CreateMap<ServiceDto, Service>();

            CreateMap<BookComponent, BookComponentDto>();
            CreateMap<BookComponentDto, BookComponent>();

            CreateMap<OrderBook, OrderBookDto>();
            CreateMap<OrderBookDto, OrderBook>();

            CreateMap<InProductionRW, InProductionRWDto>();
            CreateMap<InProductionRWDto, InProductionRW>();

            CreateMap<InProductionRW, SubOrderDto>();
            CreateMap<SubOrderDto, InProductionRW>();

            CreateMap<WarehouseItem, WarehouseItemDto>();
            CreateMap<WarehouseItemDto, WarehouseItem>();

            CreateMap<ReservedItem, ReservedItemDto>();
            CreateMap<ReservedItemDto, ReservedItem>();
        }
        #endregion
    }
}
