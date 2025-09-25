namespace StarterKit.Application.Consts
{
    public class AllDto<T> where T : class
    {
        public List<T> Data { get; set; }

        public int TotalCount { get; set; }

        public int TotalPage { get; set; }
    }
}
