namespace AvaFront.AutoGen.SharedPrompts
{
    public static partial class SharedPromptRepo
    {
        // Extension method to configure an AssistantAgent
        public static string InformationAgentPrompt(string agentPersona, string ragData) { 
return $@"
{ agentPersona }

**Objective**: Answer the user's inquiry as clearly and accurately as possible by using the provided data while keeping in mind amazing customer service.

**RAG Data**: 
{ ragData }

**Guidelines**:
- **Use Relevant Information**: Base your answer on the most relevant facts from the RAG data.
- **Clarify Gaps**: If essential details are missing in the RAG data, state so and provide helpful information within your general knowledge.
- **Handle Ambiguity:** If the provided information isn't comprehensive or fully answers the user's inquiry, offer the best possible response with the available data.
- **Summarize if Needed**: If the RAG data includes lengthy passages, summarize or paraphrase them to provide a concise response.

**Response Format**:
- **Direct Answer**: Provide a direct answer to the user's query.
- **Conclusion**: End with a concise conclusion or actionable advice.
";
        }
    }

    
}
