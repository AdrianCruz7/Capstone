import requests
import json
import urllib.request
import zipfile

testSessionID = "SESSION_1697779474_3533737"

url = "https://api.csm.ai:5566/image-to-3d-sessions/get-mesh/SESSION_1697779474_3533737"
payload = ""
headers = {
  'x-api-key': 'Ee26c558fb722788e0eecd22d3bE6c5E',
}
response = requests.request("GET", url, headers=headers, data=payload)

modelFolder = response.text
modelFolderJSON = json.loads(modelFolder)
folderURL = modelFolderJSON['data'][0]['mesh_url_zip']

print(folderURL)

finalURLName = "Assets/Test/" + testSessionID + ".zip"

urllib.request.urlretrieve(folderURL, finalURLName)

#unzipping
with zipfile.ZipFile(finalURLName,"r") as zip_ref:
    zip_ref.extractall("Assets/Test/TestFolders")

#textFilePath = "Assets/Test/AssetPath.txt"
#filePathObj = "Assets/Test/TestFolders/" + testSessionID + "/mesh.obj"

#with open(textFilePath, 'w') as textSave:
#    textSave.write(filePathObj)
#    textSave.close()

tempText = "Assets/Models/ModelFolders/" + testSessionID + "/mesh.obj"

textData = {
    "textFilePath": tempText
}

saveFilePath = open("Assets/Test/AssetFilePath.json", "w")

json.dump(textData, saveFilePath, indent = 6)

saveFilePath.close

print("almost there")