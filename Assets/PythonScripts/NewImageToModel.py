import os
import secrets
from token import NUMBER
from turtle import back
import openai
import boto3
import urllib.request
import requests
import json
import time
import zipfile
def createFile(fileName, content):
    #filepathdata
    FPD = {
        "textFilePath": content
    }
    
    saveFile = open(fileName, "w")
    
    json.dump(FPD, saveFile, indent=6)
    
    saveFile.close()

#open files
file = open('Assets/Editor/LastPrompt.json')
fileKey1 = open('Assets/Keys/OpenAIKey.json')
fileKey2 = open('Assets/Keys/MeshyKey.json')

#grab data from file
data = json.load(file)
dataKey1 = json.load(fileKey1)
dataKey2 = json.load(fileKey2)

#grabs prompt
test1 = data.get('prompt')

#closes files
file.close()
fileKey1.close()
fileKey2.close()

#aws variables
awsAccessKeyId = "AKIAYAO4I4IRY2NXGMW5"
awsSecretAccessKey = "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"
bucketName = "texttomodelbucket"

#connecting to container
s3 = boto3.client('s3', aws_access_key_id=awsAccessKeyId, aws_secret_access_key=awsSecretAccessKey)

#OpenAI key
#openai.api_key = "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"
openai.api_key = dataKey1.get('key')

#create image
createdImage = openai.Image.create(
    prompt=test1,
    n=1,
    size="512x512"
)

#grabs url from OpenAI
imageURL = createdImage['data'][0]['url']

#generates a random name for the file
imageName = "Assets/ImagesFolder/" + secrets.token_urlsafe(16) + ".png"

#create filepath for reference image
createFile("Assets/Models/RefImageFilePath.json", imageName)

#downloads it into a png
urllib.request.urlretrieve(imageURL, imageName)

#"local" path
localPath = imageName

#folder path for container
s3Key = "testFolder/{}".format(imageName)

try:
    s3.upload_file(localPath, bucketName, s3Key)
    print(f"Image uploaded to S3: s3://{bucketName}/{s3Key}\n")
except Exception as e:
    print(f"An error occurred: {str(e)}")

#url to the image in aws
awsURL = "https://s3.amazonaws.com/{}/{}".format(bucketName, s3Key)

#NEW MESHY STUFF
#meshyKey = "msy_04yhKpnkOmf23e79NskCIm0kp4BiwmG7iU0o"
meshyKey = dataKey2.get('key')

payload = {
    "image_url": awsURL,
    "enable_pbr": True,
}

headers = {
    "Authorization": f"Bearer {meshyKey}"
}

response = requests.post(
    "https://api.meshy.ai/v1/image-to-3d",
    headers=headers,
    json=payload,
)

response.raise_for_status()

#places json in a new variable
jsonOutput = response.json()

#grabs and stores sessionID
sessionID = jsonOutput['result']

#GET MESHY STUFF
failSafe = 0
status = ""

while(status != "SUCCEEDED" and failSafe < 10):
    failSafe += 1
    time.sleep(60)
    
    response = requests.get(
    f"https://api.meshy.ai/v1/image-to-3d/{sessionID}",
    headers=headers,
    )
    
    response.raise_for_status()
    
    jsonOutput = response.json()
    
    status = jsonOutput['status']
    print(status)

print(jsonOutput)

#grabs the model url
meshURL = jsonOutput['model_urls']['glb']
#meshURL = jsonOutput['model_url']
baseColorURL = jsonOutput['texture_urls'][0]['base_color']

numberTest = secrets.token_urlsafe(10)

#filepath of the folder
#finalPathName = "Assets/Models/" + sessionID + ".fbx"
#finalPathName = "Assets/Models/TestMeshy/" + numberTest + "/" + sessionID + ".fbx"
finalPathName = "Assets/Models/TestMeshy/" + numberTest + "/" + sessionID + ".glb"
texturePathName = "Assets/Models/TestMeshy/" + numberTest + "/base_color.png"

os.makedirs("Assets/Models/TestMeshy/" + numberTest)

#grabs the mesh and places it in pathname
urllib.request.urlretrieve(meshURL, finalPathName)
urllib.request.urlretrieve(baseColorURL, texturePathName)

#create a filepath for asset
createFile("Assets/Models/AssetFilePath.json", finalPathName)

