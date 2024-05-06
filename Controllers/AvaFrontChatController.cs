﻿using AvaFront.API.Models;
using AvaFront.AutoGen.Agents.AvaFront;
using AvaFront.Infrastructure.CosmosDbData.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AvaFront.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvaFrontChatController : ControllerBase
    {
        ConversationHistoryRepository _conversationHistoryRepository;
        AvaFrontAgent _avaFrontAgent;
        public AvaFrontChatController(ConversationHistoryRepository conversationHistoryRepository, AvaFrontAgent avaFrontAgent)
        {
            _conversationHistoryRepository = conversationHistoryRepository;
            _avaFrontAgent = avaFrontAgent;
        }

        // POST api/<RestaurantChatController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ConversationRequest conversationRequest)
        {
            //check if conversationId is in the header
            if (!Request.Headers.TryGetValue("ConversationId", out var conversationId) || string.IsNullOrEmpty(conversationId))
            {
                var response = await _avaFrontAgent.ExecuteConversationAsync(conversationRequest.Message, null);
                response.ConversationHistory.TransformForDatabase();
                //save response to cosmos db
                var savedConversationHistory = await _conversationHistoryRepository.AddItemAsync(response.ConversationHistory);
                if (response.LastResponse == "NAVIGATETORESTAURANT" || response.LastResponse == "NAVIGATETOAUTHENTICATION" || response.LastResponse == "NAVIGATETOSALES")
                {
                    return Ok(new ConversationResponse(response.LastResponse, savedConversationHistory.id, response.LastResponse));
                } else
                {
                    return Ok(new ConversationResponse(response.LastResponse, savedConversationHistory.id));
                }                
            } else
            {
                try
                {
                    //get conversation history from db
                    var conversationHistory = await _conversationHistoryRepository.GetItemAsync(conversationId);
                    conversationHistory.TransformForApplication();
                    var response = await _avaFrontAgent.ExecuteConversationAsync(conversationRequest.Message, conversationHistory);
                    response.ConversationHistory.TransformForDatabase();
                    //save response to cosmos db
                    var savedConversationHistory = await _conversationHistoryRepository.UpdateItemAsync(conversationId, response.ConversationHistory);
                    if (response.LastResponse == "NAVIGATETORESTAURANT" || response.LastResponse == "NAVIGATETOAUTHENTICATION" || response.LastResponse == "NAVIGATETOSALES")
                    {
                        return Ok(new ConversationResponse(response.LastResponse, savedConversationHistory.id, response.LastResponse));
                    }
                    else
                    {
                        return Ok(new ConversationResponse(response.LastResponse, savedConversationHistory.id));
                    }                    
                }
                catch (Exception e)
                {
                    return BadRequest("Error handling your request. Please try again");
                }
            }
        }

    }
}
