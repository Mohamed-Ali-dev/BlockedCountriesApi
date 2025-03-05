namespace Infrastructure.DTOs
{
    public class FilterBlockedCountriesDTO
    {
        public int Page { get; set; } = 1;
        private int pageSize = 10;
        private readonly int maxPageSize = 50;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize ? maxPageSize : value); }
        }
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; } 
    }
}
