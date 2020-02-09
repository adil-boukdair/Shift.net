
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiftreportsLib.AWS
{
	public class S3
	{

		string accessKey = "AKIAIEEOK7JGCQTDMXEQ";
		string secretAccessKey = "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";
		
		string s3Bucket = "submitedreports";
		string serviceUrl = "http://s3-external-1.amazonaws.com";  //N. Virginia service url         
		string newFileName = "-" + DateTime.Now.Ticks.ToString() + ".json"; //new filename in s3, optional



		Amazon.S3.AmazonS3Client S3Client = null;

		public S3(string accessKeyId, string secretAccessKey, string serviceUrl)
		{
			Amazon.S3.AmazonS3Config s3Config = new Amazon.S3.AmazonS3Config();
			s3Config.ServiceURL = serviceUrl;

			this.S3Client = new Amazon.S3.AmazonS3Client(accessKeyId, secretAccessKey, s3Config);

			
		}


		public S3()
		{
			Amazon.S3.AmazonS3Config s3Config = new Amazon.S3.AmazonS3Config();
			s3Config.ServiceURL = serviceUrl;

			this.S3Client = new Amazon.S3.AmazonS3Client(accessKey, secretAccessKey, s3Config);


		}






		public void UploadFile(string filePath, string bucketsubfolder, string newFileName, bool deleteLocalFileOnSuccess)
		{
			//save in s3
			Amazon.S3.Model.PutObjectRequest s3PutRequest;
			s3PutRequest = new Amazon.S3.Model.PutObjectRequest();
			s3PutRequest.FilePath = filePath;
			s3PutRequest.BucketName = s3Bucket+@"/"+ bucketsubfolder;
			s3PutRequest.CannedACL = Amazon.S3.S3CannedACL.PublicReadWrite;

			//key - new file name
			if (!string.IsNullOrWhiteSpace(newFileName))
			{
				s3PutRequest.Key = newFileName+".json";
			}

			s3PutRequest.Headers.Expires = new DateTime(2020, 1, 1);

			try
			{
				Amazon.S3.Model.PutObjectResponse s3PutResponse = this.S3Client.PutObject(s3PutRequest);

				if (deleteLocalFileOnSuccess)
				{
					//Delete local file
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}
			}
			catch (Exception ex)
			{
				//handle exceptions
			}
		}


		public void UploadString(string Content, string s3Bucket, string newFileName, bool deleteLocalFileOnSuccess)
		{


			String tmpPath = System.IO.Path.GetTempFileName();
			System.IO.File.AppendAllText(tmpPath, Content);
			UploadFile(tmpPath, s3Bucket, "sr-"+ newFileName, deleteLocalFileOnSuccess);
		}

	}
}