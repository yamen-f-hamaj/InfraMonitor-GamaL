# 🚀 InfraMonitor CI/CD Pipeline Documentation

## 📌 Overview

This document describes the GitHub Actions CI/CD pipeline used to build and publish Docker images for the InfraMonitor system.

The pipeline automatically:

* Builds the **WebAPI Docker image**
* Builds the **Angular frontend image**
* Pushes images to **GitHub Container Registry (GHCR)**
* Runs on every push to the `main` branch

---

## 🏗️ Architecture

The pipeline is implemented using **GitHub Actions** and Docker Buildx.

### Images Produced

| Service  | Image Name                              |
| -------- | --------------------------------------- |
| Web API  | `ghcr.io/<owner>/inframonitorwebapi`    |
| Frontend | `ghcr.io/<owner>/inframonitor-frontend` |

---

## 🔄 Pipeline Trigger

The workflow runs on:

```yaml
on:
  push:
    branches: [ "main" ]
  workflow_dispatch:
```

### Trigger Types

* ✅ Automatic on push to `main`
* ✅ Manual run from GitHub Actions UI

---

## ⚙️ Workflow Location

```
.github/workflows/docker-build.yml
```

---

## 🧩 Pipeline Steps

### 1️⃣ Checkout Source Code

The pipeline first checks out the repository.

**Action used:**

```
actions/checkout@v4
```

---

### 2️⃣ Authenticate with GHCR

The workflow logs in to GitHub Container Registry using the built-in token.

**Action used:**

```
docker/login-action@v3
```

**Permissions required:**

```yaml
permissions:
  contents: read
  packages: write
```

---

### 3️⃣ Setup Docker Buildx

Buildx enables:

* multi-platform builds
* build caching
* faster image creation

**Action used:**

```
docker/setup-buildx-action@v3
```

---

### 4️⃣ Build and Push WebAPI Image

The WebAPI image is built using:

```
InfraMonitor.WebAPI/Dockerfile
```

**Tags produced:**

* `latest`
* `<commit-sha>`

Example:

```
ghcr.io/org/inframonitorwebapi:latest
ghcr.io/org/inframonitorwebapi:<sha>
```

---

### 5️⃣ Build and Push Frontend Image

The Angular frontend image is built from:

```
../../GamaFront-end/InfraMonitor/Dockerfile
```

**Tags produced:**

* `latest`
* `<commit-sha>`

---

## 🔐 Secrets and Authentication

This pipeline uses:

* ✅ `GITHUB_TOKEN` (automatic)
* ❌ No Docker Hub credentials required

### Required Permissions

Ensure workflow has:

```yaml
permissions:
  contents: read
  packages: write
```

---

## 🐳 Deployment Usage

After the pipeline runs, update production `docker-compose` to use the built images:

```yaml
inframonitor.webapi:
  image: ghcr.io/<owner>/inframonitorwebapi:latest
```

**Important:** Remove the `build:` section in production.

---

## 🚨 Known Considerations

### ⚠️ Frontend Build Context

Current configuration:

```
../../GamaFront-end/InfraMonitor
```

This requires that:

* the frontend exists in the same repository, OR
* the pipeline is modified to checkout another repo

---

### 🔒 Security Recommendations

For production environments:

* ❗ Move database passwords to secrets
* ❗ Avoid committing credentials
* ❗ Use environment-specific compose files
* ❗ Enable image vulnerability scanning

---

## 📈 Future Improvements

Recommended enhancements:

* Add unit test stage
* Add integration tests
* Enable Docker layer caching
* Add semantic version tagging
* Add staging deployment
* Add production deployment
* Add health checks

---

## 🧪 Manual Pipeline Run

To manually trigger:

1. Go to **GitHub → Actions**
2. Select workflow
3. Click **Run workflow**

---

## ✅ Success Criteria

A successful run will:

* Build both images
* Push to GHCR
* Show green status in Actions
* Make images available for deployment

---

## 👨‍💻 Maintainer Notes

This pipeline follows CI/CD best practices:

* immutable image tagging
* registry-based deployment
* separation of build and runtime
* container-first strategy
