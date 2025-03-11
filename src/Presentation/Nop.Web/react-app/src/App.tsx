import {useState} from "react";

export interface DropdownProps {
    name: string;
    icon: string;
    options: string[];
}

const SearchForm: React.FC<{ dropdowns: DropdownProps[] }> = (props) => {
    const [activeTab, setActiveTab] = useState("New");
    const [make, setMake] = useState("");
    const handleClick = (tab: string) => {
        setActiveTab(tab);
    };
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
                    <div className="col-lg-2">
                        <div className="input-group border-end-lg border-light">
              <span className="input-group-text text-muted ps-2 ps-sm-3">
                <i className="fi-search"></i>
              </span>
                            <input className="form-control" type="text" name="keywords" placeholder="Keywords..."/>
                        </div>
                    </div>
                    <hr className="hr-light d-lg-none my-2"/>

                    {/* Dropdowns */}
                    {
                        props.dropdowns
                            .map((dropdown, index) => (
                                <div key={index} className="col-lg-2 col-md-3 col-sm-6">
                                    <div className="dropdown border-end-sm border-light" data-bs-toggle="select">
                                        <button className="btn btn-link dropdown-toggle ps-2 ps-sm-3" type="button"
                                                data-bs-toggle="dropdown">
                                            <i className={dropdown.icon + " me-2"}></i>
                                            <span className="dropdown-toggle-label">{dropdown.name}</span>
                                        </button>
                                        <input type="hidden" name={dropdown.name.toLowerCase()}/>
                                        <ul className="dropdown-menu dropdown-menu-dark">
                                            {dropdown.options.map((option, idx) => (
                                                <li key={idx}>
                                                    <a className="dropdown-item" href="#"
                                                       onClick={event => {
                                                           event.preventDefault();
                                                           setMake(option)
                                                       }}>
                                                        <span className="dropdown-item-label">{option}</span>
                                                    </a>
                                                </li>
                                            ))}
                                        </ul>
                                    </div>
                                </div>
                            ))}

                    <hr className="hr-light d-lg-none my-2"/>
                    <div className="col-lg-2">
                        <button className="btn btn-primary w-100" type="button" onClick={(event) => {
                            event.preventDefault();
                            window.location.href = `https://localhost:5001/${make}`
                        }}>Search</button>
                    </div>
                </div>
            </form>
        </>
    );
};

export default SearchForm;
