import {useCallback, useEffect, useState} from "react";
import FormItem from "./FormItem.tsx";

export interface DropdownProps {
    Name: string;
    Icon: string;
    Options: { Name: string; SeoName: string | null; Id: number; }[];
}

const SearchForm: React.FC<{ dropdowns: DropdownProps[] }> = (props) => {
    const [activeTab, setActiveTab] = useState("New");
    const [seoName, setSeoName] = useState<string | null>(null);
    const [makeId, setMakeId] = useState<number>();
    const [make] = useState<DropdownProps>(props.dropdowns[0]);
    //const [isDropdownVisible, setIsDropdownVisible] = useState(false);
    const [model, setModel] = useState<DropdownProps>(props.dropdowns[1]);
    const [bodyType] = useState<DropdownProps>(props.dropdowns[2]);
    const [location] = useState<DropdownProps>(props.dropdowns[3]);
    
    const handleClick = (tab: string) => {
        setActiveTab(tab);
    };

    useEffect(() => {
        (async () => {
            if (makeId === undefined) return; // ✅ Prevent API call when make is null
            try {
                const response = await fetch("https://localhost:5001/SellCar/ChildrenCategories", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({id: makeId}),
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                const result: { Text: string; Value: string }[] = await response.json(); // Ensure correct API response type

                // ✅ Convert API response to match `Options` type
                const options: { Name: string; SeoName: string | null; Id: number }[] = result.map((item) => ({
                    Name: item.Text, // Use API's "Text" as "Name"
                    SeoName: null,
                    Id: Number(item.Value) // Ensure `Id` is a number
                }));

                // ✅ Ensure the function returns a valid `DropdownProps[]`
                setModel((prevModel) => ({
                    ...prevModel, // Spread existing properties (Name, Icon)
                    Name: "Model",
                    Options: options // Update only Options
                }));
            } catch (err) {
                //setError(err.message);
                console.error("Fetch Error:", err);
            }
        })();
    }, [makeId]);

    const handleMakeSelect = useCallback((option: { Name: string; SeoName: string | null; Id: number }) => {
        setSeoName(option.SeoName)
        setMakeId(option.Id);
    }, []);
    const handleModelSelect = useCallback((option: { Name: string; SeoName: string | null; Id: number }) => {
        setSeoName(option.SeoName)
        setModel((prevModel) => ({
            ...prevModel,
            Name: option.Name,
        }));
    }, []);
    
    return (
        <>
            {/* Tabs */}
            <ul className="nav nav-tabs nav-tabs-light mb-4">
                <li className="nav-item">
                    <a className={`nav-link${activeTab === "New" ? " active" : ""}`} href="#" onClick={(e) => {
                        e.preventDefault();
                        handleClick("New");
                    }}>New</a>
                </li>
                <li className="nav-item">
                    <a className={`nav-link${activeTab === "Used" ? " active" : ""}`} href="#" onClick={(e) => {
                        e.preventDefault();
                        handleClick("Used");
                    }}>Used</a>
                </li>
            </ul>

            {/* Form group */}
            <form className="form-group form-group-light d-block">
                <div className="row g-0 ms-lg-n2">
                    {/* Keywords Input */}
                    <div className="col-lg-2">
                        <div className="input-group border-end-lg border-light">
                        <span className="input-group-text text-muted ps-2 ps-sm-3">
                            <i className="fi-search"></i>
                        </span>
                            <input className="form-control" placeholder="Keywords..." type="text" name="keywords"/>
                        </div>
                    </div>
                    <hr className="hr-light d-lg-none my-2"/>
                    {/* Dynamic Dropdowns */}
                    <FormItem item={make} style={"fi-list"} onPress={handleMakeSelect}/>
                    <FormItem item={model} style={"fi-list"} onPress={handleModelSelect}/>
                    <FormItem item={bodyType} style={"fi-car fs-lg"} onPress={useCallback(() => {}, [])}/>
                    <FormItem item={location} style={"fi-map-pin"} onPress={useCallback(() => {}, [])}/>
                    <hr className="hr-light d-lg-none my-2"/>
                    {/* Search Button */}
                    <div className="col-lg-2">
                        <button className="btn btn-primary w-100" type="button" disabled={!seoName} onClick={() => window.location.href = `https://localhost:5001/${seoName}`}>
                            Search
                        </button>
                    </div>
                </div>
            </form>
        </>
    );
};

export default SearchForm;
