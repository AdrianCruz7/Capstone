import secrets
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

#open file
file = open('Assets/Editor/LastPrompt.json')
fileKey1 = open('Assets/Keys/OpenAIKey.json')
fileKey2 = open('Assets/Keys/CSMKey.json')

#grab data from file
data = json.load(file)
dataKey1 = json.load(fileKey1)
dataKey2 = json.load(fileKey2)

#grabs prompt
test1 = data.get('prompt')

#closes file
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
    size="256x256",
)

#grabs url from OpenAI
imageURL = createdImage['data'][0]['url']

#generates a random name for the file
imageName = "Assets/ImagesFolder/" + secrets.token_urlsafe(16) + ".png"

#create filepath for reference image
createFile("Assets/Models/RefImageFilePath.json", imageName)

#debug
#print(imageName + "\n")

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

#3dmodel stuff
url1 = "https://api.csm.ai:5566/image-to-3d-sessions"

payload = json.dumps({
  "image_url": awsURL,
  "auto_gen_3d":"true"
})

headers = {
  'x-api-key': 'Ee26c558fb722788e0eecd22d3bE6c5E',
  'Content-Type': 'application/json'
}

#makes the model
response = requests.request("POST", url1, headers=headers, data=payload)

#stores the data from the post
modelData = response.text

#turns data into a json file to grab more easily
modelDataJSON = json.loads(modelData)
sessionID = modelDataJSON['data']['session_code']

#variables
failsafe = 0
folderURL = ""

#loops until it hits the failsafe, or gets the preview model
while(folderURL == "" and failsafe < 15):
    failsafe += 1
    time.sleep(150)
    
    url2 = "https://api.csm.ai:5566/image-to-3d-sessions/get-mesh/" + sessionID

    newPayload = ""

    newHeaders = {
      'x-api-key': 'Ee26c558fb722788e0eecd22d3bE6c5E',
    }
    
    newResponse = requests.request("GET", url2, headers=newHeaders, data=newPayload)
    modelFolder = newResponse.text

    modelFolderJSON = json.loads(modelFolder)
    folderURL = modelFolderJSON['data'][0]['preview_mesh_url_zip']

#filepath of the folder
finalURLName = "Assets/Models/" + sessionID + ".zip"

#grabs the zip file and puts in the assets/models folder
urllib.request.urlretrieve(folderURL, finalURLName)

#unzipping
with zipfile.ZipFile(finalURLName,"r") as zip_ref:
    zip_ref.extractall("Assets/Models/ModelFolders")

#create filepath name for session mesh
tempText = "Assets/Models/ModelFolders/" + sessionID + "/mesh.obj"

#create a filepath for asset
createFile("Assets/Models/AssetFilePath.json", tempText)

print("almost there")