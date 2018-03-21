import binascii
filename = 'Path\\To\\File\\Cert.der'
with open(filename, 'rb') as f:
    content = f.read()

hexData = binascii.hexlify(content)
hexList = list(''.join(map(chr,hexData)))
outString = ''
caCertLen = 0

x = len(hexList)
for i in range(0, (x-1), 2):
    first = hexList[i]
    second = hexList[i+1]
    outString = outString + '0x' + first + second + ', '
    caCertLen = caCertLen + 1

outString = outString[:-2] #remove last comma and space

print(outString)
print('Cert length = ' + str(caCertLen))