// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.17.1

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly IConfiguration _configuration;

        public EchoBot(IConfiguration c)
        {
            _configuration = c;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var openAiKey = _configuration["OpenAi"] ?? Environment.GetEnvironmentVariable("OpenAi");
            var client    = new RestClient("https://api.openai.com/v1/completions");
            var request   = new RestRequest();
            request.AddHeader("Content-Type",  "application/json");
            request.AddHeader("Authorization", "Bearer " + openAiKey);

            var requestBody = new
                              {
                                  model             = "text-davinci-003",
                                  prompt            = turnContext.Activity.Text,
                                  temperature       = 0.9,
                                  max_tokens        = 4000,
                                  top_p             = 1,
                                  frequency_penalty = 0,
                                  presence_penalty  = 0.6,
                                  stop              = new string[] { " Human:", " AI:" }
                              };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody),
                                 ParameterType.RequestBody);

            var response        = await client.ExecutePostAsync(request, cancellationToken: cancellationToken);
            var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
            var generatedText   = responseContent.choices[0].text.ToString().Trim();

            await turnContext.SendActivityAsync(MessageFactory.Text(generatedText), cancellationToken);
        }
    }
}