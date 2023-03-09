# GitOps Configuration

Coming soon...

[Back](../README.md)
<!-- #### Kustomize

[Kustomize](https://kubernetes.io/docs/tasks/manage-kubernetes-objects/kustomization/) is a tool for customizing Kubernetes configurations by:

- Generating resources from other sources

- Setting cross-cutting fields for resources

- Composing and customizing collections of resources

Every folder that is relevant to the Flux configuration contains a `kustomization.yaml` file. These files define which resources will be considered as part of the Kustomize configuration and whether any patching is required for a more specific configuration. 

Below is the content of `apps/kustomization.yaml`, it instructs Flux to look into the `voteapp` folder for additional Kustomization references, which can define objects like **HelmRepository**, **HelmRelease** or simply **Kubernetes manifests**.

```yaml
## apps/kustomization.yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization
resources:
  - voteapp
```

The folder `sources` contains [HelmRepository](https://fluxcd.io/docs/components/source/helmrepositories/) objects that will be used in the solution. The `HelmRepository` resource specifies the location of a [Helm Chart](https://helm.sh/). Those charts can be located at an OCI Helm repository (like [like Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-image-formats#oci-images)) or an HTTP/HTTPS repository like Git (follow this great [article](https://medium.com/@mattiaperi/create-a-public-helm-chart-repository-with-github-pages-49b180dbb417) to create a public Helm chart repository with GitHub Pages and look at the [helm-package.yaml](../../.github/workflows/helm-package.yaml) workflow to learn how to package and index your Helm charts in GitHub).

> **NOTE:** For secret repositories, your `HelmRepository` you must provide a [Secret reference](https://fluxcd.io/docs/components/source/helmrepositories/#secret-reference).

The folders `app/voteapp`, `infrastructure/ingress-nginx` and `infrastructure/redis` contain the Kustomize configurations for each app or release. They all define (for the most part) the same Kubernetes resources:

- [Namespaces](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/) objects define logical boundaries to isolate groups of resources within a cluster. In our case, each release will have a namespace for itself.

- [HelmRelease](https://fluxcd.io/docs/components/helm/helmreleases/) objects define a resource that will be reconciliated via Helm actions such as install, upgrade, test, uninstall and rollback. The `.spec` object expects almost the same parameters as the `helm install/upgrade` commands: the target namespace to install the release, the repo location (in this case, referencing a `HelmRepository` object), and override values for the release if needed.

- [Kustomization](https://kubectl.docs.kubernetes.io/references/kustomize/glossary/#kustomization) objects specify all the resources that will be used by Kustomize. Below is the content of `infrastructure/ingress-nginx/kustomization.yaml`, and despite having other files in the same folder, Kustomize will ignore them unless they are added as a Kustomization resource.

```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization
resources:
 - repository.yaml
 - namespace.yaml
 - release.yaml
```

> **IMPORTANT:** `ingress-nginx` and `redis` folders show different approaches to reconciliate their configurations. `redis` stores release values in a [ConfigMaps](https://kubernetes.io/docs/concepts/configuration/configmap/) object, while the installation of `ingress-nginx` is patched with unique information for each cluster. The next section explains how this patching is made to obtain unique Helm releases.

> **TIP:** To see the full output of a **Kustomize** configuration, use the command `kubectl kustomize <kustomization_directory>`, it will print the manifest containing all components.

##### k3s-\<uniqueId>-\<clusterIndex\>

> **NOTE:** These folders are not present in the `main` branch of the repo. They will be created during the deployment process that will be explained later in this document.

Since the ingress controller requires an IP address to listen to (passed through `controller.service.externalIPs[]`), every cluster in this sandbox deployment will require their own `HelmRelease` definition with their private IP Address.

>  **IMPORTANT:** This is a very common scenario where certain configurations require unique parameters. The way the GitOps paradigm solves it is by separating the application code and release configurations in different repositories, and pushing infrastructure and application releases to the release configuration repository as part of their CI/CD pipelines triggered by changes in the application code. This also creates security boundaries by not letting developer and IT teams tamper with the final state of the releases; they must commit their changes via their version control system which will be submitted for review and approved via pull requests, and the CI part of CI/CD will have the necessary permissions to commit changes to the release configuration repository. For simplicity's sake, this project "isolates" the release configuration for ingress controller by creating a folder for each k3s cluster and customizing its `values.yaml` file with the VM's private IP address.

#### Creating Flux Configuration in a cluster

Azure Arc for Kubernetes allows us to install extensions in the cluster so it can understand new resource types and apply their configurations. This repository install the `Microsoft.Flux` extension on each cluster and then creates three Flux configurations using the [az k8s-configuration flux](https://docs.microsoft.com/en-us/cli/azure/k8s-configuration/flux?view=azure-cli-latest) CLI extension. Let's discuss these configurations:

- Sources: It is responsible for monitoring the Kustomize configuration that contains all the `HelmRepository` objects that will be needed by `redis`, `ingress-nginx` and `voteapp`.
- NGINX: It is responsible for monitoring the Kustomize configuration that installs NGINX Ingress controller, located in the path `infrastructure/<cluster_name>` of your repository branch. As discussed previously, each cluster's ingress release requires a different IP address, resulting in separate folders for each cluster.

- Apps: It is responsible for monitoring the Kustomize configuration declared in `apps/kustomize.yaml`, which in our case only includes the `voteapp` application. But since the `voteapp` application has a dependency on a Redis cache, the Flux configuration implements two different kustomization objects. Below is a sample command to create the **apps** Flux configuration, notice how the `apps` kustomization has a dependency on `redis`, which determines the order in which Flux will create and update them.

```bash
az k8s-configuration flux create \
  -g $resourceGroupName \
  -c $clusterName \
  -n apps \
  -t connectedClusters \
  -u $repoUrl \
  --branch $repoBranch \
  --kustomization name=redis path=./infrastructure/redis \
  --kustomization name=apps path=./apps dependsOn=["redis"]
```

> **TIP:** Take a look at the [deployment script](../../deployment/azure-arc/deploy.sh) for a full view on how the configurations are created on each cluster.

Now that you have a high level understanding of the key concepts and the structure of the repository, let's move on to deploying your Azure Arc environment. -->