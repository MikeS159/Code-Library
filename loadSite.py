#MIT License(MIT)

#     CertToHex.py Version 1.0.0        #

#     Copyright(c) 2018 Mike Simpson      #

#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:

#The above copyright notice and this permission notice shall be included in all
#copies or substantial portions of the Software.

#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
#SOFTWARE.

# Must be run under python3
# How to run:
# python3 loadSite.py https://yoursite.com/sitemap_index.xml

from bs4 import BeautifulSoup
import requests
import sys

def getSiteMap(url):
	xmlDict = {}

	r = requests.get(url)
	xml = r.text

	soup = BeautifulSoup(xml, 'lxml')
	sitemapTags = soup.find_all("sitemap")

	print ("The number of sitemaps are {0}".format(len(sitemapTags)))

	for sitemap in sitemapTags:
		xmlDict[sitemap.findNext("loc").text] = sitemap.findNext("lastmod").text

	for x in xmlDict:
		print(x)
		getWebPage(x)
	

def getWebPage(url):
	xmlDict = {}

	r = requests.get(url)
	xml = r.text

	soup = BeautifulSoup(xml, 'lxml')
	sitemapTags = soup.find_all("url")
	
	print ("The number of urls are {0}".format(len(sitemapTags)))

	for sitemap in sitemapTags:
		xmlDict[sitemap.findNext("loc").text] = sitemap.findNext("lastmod").text

	for x in xmlDict:
		print(x)
		requests.get(url)
		
def main():
	print(sys.argv[1])
	print('running')
	getSiteMap(sys.argv[1])

if __name__=="__main__":
    main()