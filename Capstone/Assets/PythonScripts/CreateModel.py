import secrets
import openai
import boto3
import urllib.request
import requests
import json
import time
import zipfile
#from PIL import Image

#open file
file = open('Assets/Editor/LastPrompt.json')

#grab data from file
data = json.load(file)

#grabs prompt
test1 = data.get('prompt')

#debug
#print(test1)

#closes file
file.close()

#debug
#print("Test")

#aws stuff
awsAccessKeyId = "AKIAYAO4I4IRY2NXGMW5"
awsSecretAccessKey = "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"
bucketName = "texttomodelbucket"

#bucket stuff
s3 = boto3.client('s3', aws_access_key_id=awsAccessKeyId, aws_secret_access_key=awsSecretAccessKey)

#OpenAI stuff
openai.api_key = "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"

#create image
createdImage = openai.Image.create(
    prompt=test1,
    n=1,
    size="1024x1024",
)

#debug
#print(createdImage)

#grabs url from OpenAI
imageURL = createdImage['data'][0]['url']

#debug
#print(imageURL + "\n")

#generates a random name for the file
imageName = "Assets/ImagesFolder/" + secrets.token_urlsafe(16) + ".png"

filePathData = {
    "textFilePath": imageName
}

saveImageFilePath = open("Assets/Models/RefImageFilePath.json", "w")

json.dump(filePathData, saveImageFilePath, indent=6)

saveImageFilePath.close()

#debug
#print(imageName + "\n")

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
#print(awsURL + "\n")

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
print(response.text)
print("")

#turns data into a json file to grab more easily
modelDataJSON = json.loads(modelData)
sessionID = modelDataJSON['data']['session_code']

#variables
failsafe = 0
folderURL = ""

#debug
#testSessionID = "SESSION_1696460520_5958833"

#loops until it hits the failsafe, or gets the preview model
while(folderURL == "" and failsafe < 30):
    failsafe += 1
    time.sleep(150)
    
    url2 = "https://api.csm.ai:5566/image-to-3d-sessions/get-mesh/" + sessionID

    newPayload = ""

    newHeaders = {
      'x-api-key': 'Ee26c558fb722788e0eecd22d3bE6c5E',
    }
    
    newResponse = requests.request("GET", url2, headers=newHeaders, data=newPayload)
    modelFolder = newResponse.text
    
    print(modelFolder)
    print("")

    modelFolderJSON = json.loads(modelFolder)
    folderURL = modelFolderJSON['data'][0]['preview_mesh_url_zip']

print(folderURL + "/n")

#filepath of the folder
finalURLName = "Assets/Models/" + sessionID + ".zip"

#grabs the zip file and puts in the assets/models folder
urllib.request.urlretrieve(folderURL, finalURLName)

#unzipping
with zipfile.ZipFile(finalURLName,"r") as zip_ref:
    zip_ref.extractall("Assets/Models/ModelFolders")

#textFilePath = "Assets/Models/AssetPath.txt"
#filePathObj = "Assets/Models/ModelFolders/" + sessionID + "/mesh.obj"

tempText = "Assets/Models/ModelFolders/" + sessionID + "/mesh.obj"

textData = {
    "textFilePath": tempText
}

#with open(textFilePath, 'w') as textSave:
#    textSave.write(filePathObj)
#    textSave.close()

saveFilePath = open("Assets/Models/AssetFilePath.json", "w")

json.dump(textData, saveFilePath, indent = 6)

saveFilePath.close()

print("almost there")

