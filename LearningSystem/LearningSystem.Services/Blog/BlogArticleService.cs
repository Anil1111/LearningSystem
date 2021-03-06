﻿namespace LearningSystem.Services.Blog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
    using Models;
    using Repository.Contracts;
    using Utilities.Common;

    public class BlogArticleService : IBlogArticleService
    {
        private readonly IRepository<LearningSystemContext, Article> repository;
        private readonly IMapper mapper;

        public BlogArticleService(IRepository<LearningSystemContext, Article> repository,
            IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public IEnumerable<TModel> AllArticles<TModel>(int page = 1)
        {
            var articles = this.repository
                .Get()
                .OrderByDescending(a => a.PublishDate)
                .Skip((page - 1) * 25)
                .Take(25);

            var model = this.mapper.Map<IEnumerable<TModel>>(articles);

            return model;
        } 

        public async Task CreateArticleAsync<TModel>(TModel model, string authorId)
        {
            CoreValidator.EnsureNotNull(model, "The article is null");
            var article = this.mapper.Map<Article>(model);
            article.AuthorId = authorId;
            article.PublishDate = DateTime.UtcNow;

            await this.repository.AddAsync(article);
        }

        public async Task<int> TotalAsync() => await this.repository.GetCountAsync();
    }
}