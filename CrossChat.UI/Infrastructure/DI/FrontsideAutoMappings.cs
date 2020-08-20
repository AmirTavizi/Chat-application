using AutoMapper;

using CrossChat.UI.ViewModels;

namespace CrossChat.UI.Infrastructure
{
    public class FrontsideAutoMappings : Profile
    {
        public FrontsideAutoMappings():base()
        {
            //CreateMap<UserProfileViewModel, AddEditUserData>()
            //.ForMember(x => x.DisplayName, x => x.MapFrom(y => y.UserDisplayName));

            //CreateMap<User, UserTestSessionInfo>().ForMember(x => x.UserId, x => x.MapFrom(y => y.Id));

            //CreateMap<AddEditUserData, UserProfileViewModel>();
            //CreateMap<User, UserProfileViewModel>().ForMember(x => x.UserId, x => x.MapFrom(y => y.Id));
            //CreateMap<RegisterViewModel, AddEditUserData>();

            //CreateMap<DemandListItem, DemandListItemData>()
            //    .ForMember(x=>x.ProductCategory, x=>x.MapFrom(y=>y.ProductCategory.Name))
            //    .ForMember(x => x.Unit, x => x.MapFrom(y => y.Unit.Name));

            //CreateMap<DemandListCriteria, DemandListCriteriaData>();

            //CreateMap<SubsribeViewModel, AddEditSubscriptionData>();

            //CreateMap<AddEditSubscriptionData, SubsribeViewModel>();

            //CreateMap<CompanyViewModel, AddEditCompanyData>();

            //CreateMap<AddEditCompanyData, CompanyViewModel>();

            //CreateMap<CompanyUserViewModel, AddEditCompanyUserData>();

            //CreateMap<AddEditCompanyUserData, CompanyUserViewModel>();

            ////////////////Emir/////////////////////////////////////
            //CreateMap<AdminUser, AdminUserViewModel>();
            //CreateMap<AdminUserViewModel, AddEditAdminUserData>();

            //CreateMap<AdminUser, AdminUserSessionInfo>().ForMember(x => x.UserId, x => x.MapFrom(y => y.Id));
            ////////////////Emir////////////////////////////////

            //CreateMap<AddEditTemperamentData, AddEditTemperamentViewModel>();
           // CreateMap<AddEditTemperamentViewModel, AddEditTemperamentData>();

           // CreateMap<AddEditReportData, AddEditReportViewModel>();
           // CreateMap<AddEditReportViewModel, AddEditReportData>();



            //CreateMap<UserSessionInfo, User > ()
            //  .ForMember(x => x.Id, x => x.MapFrom(y => y.UserId));

            //CreateMap<RegisterViewModel, AddEditUserData>();
            //CreateMap<MyUserProfileViewModel, User>();
            //CreateMap<User, MyUserProfileViewModel>();

            //CreateMap<MyUserProfileViewModel, AddEditUserData>();





            //CreateMap<AddEditCompanyDeliveryOptionData, AddEditDeliveryOptionViewModel>();
            //CreateMap<AddEditDeliveryOptionViewModel,AddEditCompanyDeliveryOptionData>();

            //CreateMap<CompanyDeliveryOption, AddEditCompanyDeliveryOptionData>()
            //    .ForMember(x => x.SelectedCityIds, x => x.MapFrom(y => y.Cities.Select(z=>z.CityId)));

            //CreateMap<ShowcaseSearchProductCriteria, SearchProductCriteria>();
            //CreateMap<SearchProductCriteria, ShowcaseSearchProductCriteria>();



        }
    }
}
