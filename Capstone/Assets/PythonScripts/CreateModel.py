import os
import secrets
import openai
import boto3
import urllib.request
import requests
import json
#from PIL import Image

#insert key code here

#create image
createdImage = openai.Image.create(
    prompt="a low polygon tree",
    n=1,
    size="1024x1024",
)

#grabs url from OpenAI
imageURL = createdImage['data'][0]['url']

#debug
print(imageURL + "\n")

#generates a random name for the file
imageName = "Assets/ImagesFolder/" + secrets.token_urlsafe(16) + ".png"

#debug
print(imageName + "\n")

#downloads it into a png
urllib.request.urlretrieve(imageURL, imageName)

#"local" path
localPath = imageName

#folder path for bucket
s3Key = "testFolder/{}".format(imageName)

try:
    s3.upload_file(localPath, bucketName, s3Key)
    print(f"Image uploaded to S3: s3://{bucketName}/{s3Key}\n")
except Exception as e:
    print(f"An error occurred: {str(e)}")

#url to the image in aws
awsURL = "https://s3.amazonaws.com/{}/{}".format(bucketName, s3Key)

#debug
print(awsURL + "\n")


#3dmodel stuff
url1 = "https://api.csm.ai:5566/image-to-3d-sessions"

payload = json.dumps({
  "image_url": awsURL,
  "auto_gen_3d":"true"
})

headers = {
  'x-api-key': 'Ee26c558fb722788e0eecd22d3bE6c5E',
  'Content-Type': 'application/json'
}

response = requests.request("POST", url1, headers=headers, data=payload)
print(response.text)

print("success?")