pipeline {
    agent any
    environment {
        AWS_ACCOUNT_ID="162387011843"
        AWS_DEFAULT_REGION="ap-southeast-1"
        IMAGE_REPO_NAME="gamification-service"
        IMAGE_TAG="latest"
        REPOSITORY_URI = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com/${IMAGE_REPO_NAME}"
    }
  
    stages {   
      stage('Logging into AWS ECR') {
            steps {
                script {
                sh 'docker login --username AWS -p $(aws ecr get-login-password --region ap-southeast-1) 162387011843.dkr.ecr.ap-southeast-1.amazonaws.com'  
                }
                
            }
        }
        stage('Cloning Git') {
            steps {
                checkout([$class: 'GitSCM', branches: [[name: '*/master']], doGenerateSubmoduleConfigurations: false, extensions: [], submoduleCfg: [], userRemoteConfigs: [[credentialsId: '', url: 'https://github.com/edtechse/Gamification-service-1.git']]])    
            }
        }
    // Building Docker images
    stage('Building image') {
      steps{
        script {
          dockerImage = docker.build "${IMAGE_REPO_NAME}:${IMAGE_TAG}"
        }
      }
    }
  
    // Uploading Docker images into AWS ECR
    stage('Pushing to ECR') {
     steps{
         script {
                sh "docker tag ${IMAGE_REPO_NAME}:${IMAGE_TAG} ${REPOSITORY_URI}:$IMAGE_TAG"
                sh "docker push ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com/${IMAGE_REPO_NAME}:${IMAGE_TAG}"
         }
        }
      } 
    }
      post {
        failure {
            emailext body: '\'Build failed in Jenkins: $PROJECT_NAME - #$BUILD_NUMBER\'', replyTo: 'no-reply@gmail.com', subject: '\'$PROJECT_NAME - Build # $BUILD_NUMBER - ERROR!\'', to: 'edtechse@gmail.com, likhariinder@gmail.com'
        }
    }
}
