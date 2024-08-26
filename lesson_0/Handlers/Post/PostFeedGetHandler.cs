namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;

    public partial class PostFeedGetHandler : IRequestHandler<PostFeedGetRequest, PostModel[]>
    {
        private readonly IMemoryCache _cache;
        public PostFeedGetHandler(ILifetimeScope scope)
        {
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task<PostModel[]> Handle(PostFeedGetRequest request, CancellationToken cancellationToken)
        {
            try
            {                
                _cache.TryGetValue<List<PostModel>>(request.UserId.ToString(), out var feed);

                if (feed == null)
                {
                    return [];
                }
                else
                {
                    if (feed.Count <= request.Offset)
                    {
                        return feed.ToArray();
                    }
                    else
                    {
                        return feed.Skip(request.Offset).Take(request.Limit).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }
    }
}
