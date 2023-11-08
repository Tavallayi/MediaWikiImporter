import { Counter } from "./components/Counter";
import DownloadResources from "./components/DownloadResources";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import WantedFiles from "./components/WantedFiles";
import WantedPages from "./components/WantedPages";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/wantedfiles',
        element: <WantedFiles />
    },
    {
        path: '/wantedpages',
        element: <WantedPages />
    //},
    //{
    //    path: '/resources',
    //    element: <DownloadResources resourceType="WantedResourcesTest" resources={['qwe', 'asd']} />
    }
];

export default AppRoutes;
