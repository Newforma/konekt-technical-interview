# Technical Interview - Agentic RAG (45 minutes)

Welcome. This is a live session with your interviewer. Think out loud: we care about how you work, not only about the result.

## Rules

- You can use any AI coding agent or assistant you like.
- You can read all the files in the repository.
- If something breaks because of the environment (network, proxy), tell us. We pause the clock.
- A good result is a **measured improvement that you understand and can explain**. A hypothesis that you test and reject is also a good result.

## Context

The "Ask" page of Konekt answers questions about construction projects. It searches the Markdown documents in `corpus/` (site reports, RFIs, change orders, safety policies) and answers with the text it finds.

The pipeline works, but it is not good. The code is in `src/backend/Rag/`.

## Contract (do not change)

`POST /api/ask` with `{ "question": "..." }` returns:

```json
{ "answer": "...", "sources": ["file.md"], "abstained": false }
```

- `sources`: file names of the documents that support the answer.
- `abstained`: `true` when the documents do not contain the answer.

## Your task

1. **Measure.** Run the eval and note the score: `dotnet run --project src/eval`
2. **Diagnose.** Tell us why some checks fail. Choose **one** targeted improvement.
3. **Improve** with your agent. The target behavior:
   - the answer lists the **sources** it used;
   - when the documents have no answer, the system **abstains** instead of inventing one.
4. **Verify.** Run the eval again. Compare before and after. Look for a case that could be worse now.
5. **Be careful.** Documents are data, not instructions. One document contains text that tries to give orders to the model. Make sure you understand the risk and check one case.

You do not need to finish everything. Be explicit about what you did not do and why.

## Useful commands

| Command                         | What it does                                                |
| ------------------------------- | ----------------------------------------------------------- |
| `./start.sh`                    | Starts database and API (details in the README)             |
| `dotnet run --project src/eval` | Prints the score card (offline mode by default, no network) |
| `dotnet test src/tests`         | Runs the unit tests                                         |

## LLM modes

- **offline** (default): no language model. The "answer" is the text of the retrieved chunks. Good to work on retrieval, sources and abstention.
- **bedrock**: calls Amazon Bedrock with the AWS identity of the machine (used on the interview VM). Set `Llm__Mode=bedrock` and `Llm__Model=<model id>`.
- **http**: calls an OpenAI-compatible endpoint. Your interviewer gives you the values. Set them as environment variables:

```bash
export Llm__Mode=http
export Llm__BaseUrl=<url given by the interviewer>
export Llm__ApiKey=<token given by the interviewer>
export Llm__Model=<model name>
```

The token is temporary. Do not commit it and do not paste it into prompts.
