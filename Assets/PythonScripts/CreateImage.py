import os
import openai

openai.api_key = "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"

createdImage = openai.Image.create(
    prompt="a frog playing basketball",
    n=1,
    size="1024x1024",
)

imageURL = createdImage['data'][0]['url']

print(imageURL)